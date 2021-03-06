﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;
using GameCore;

namespace Host
{
    class Program {
        public static Task<TcpClient> ClientTask;
        public static TcpListener Listener;

        public static ResourceSupplier rs { get; internal set; }

        static void Main(string[] args)
        {
            
            Console.Title = consoleTitle;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            rs = new ResourceSupplier();
            rs.Inject(new Logger(nameof(Host)));
            rs.Logger.WriteAndDisplay("海戦ゲーム：ホストサイドを起動します。");
            rs.Inject(new Game(rs));
            rs.Game.RegisterPlayer(new ConsolePlayer("You", rs));
            rs.Game.DeployShips();

            string input;
            int listenPort;
            var resultParse = false;
            do
            {
                Console.WriteLine("待ち受けポート番号（1～65535）を入力してください。デフォルト値を使用する場合には0を入力してください。");
                OutputArrow();
                input = Console.ReadLine();
                //入力が0ならデフォルト番号を使用
                resultParse = int.TryParse(input, out listenPort);
                if (resultParse && listenPort == 0)
                {
                    listenPort = defaultPort;
                    resultParse = true;
                }
                else
                {
                    resultParse = resultParse && (IPEndPoint.MinPort <= listenPort && listenPort <= IPEndPoint.MaxPort);
                }

            } while (!resultParse);

            ListenHelper helper;
            helper = new ListenHelper(listenPort);
            Listener = new TcpListener(helper.Address, helper.Port);
            try
            {
                try
                {
                    Listener.Start();
                    rs.Logger.WriteAndDisplay($"ポート番号[{listenPort}]を使用して接続要求待ち受けを開始します。");
                }
                catch (Exception)
                {
                    throw;
                }
                WaitForAccept();
                ClientService();
            }
            finally
            {
                Listener.Stop();
            }

            rs.Logger.WriteAndDisplay("海戦ゲーム：ホストサイドを終了します。");
            Console.ReadLine();
            return;
        }

        
        private async static void WaitForAccept() //I ignore this warning
        {
            ClientTask = Listener.AcceptTcpClientAsync();
            var cnt = 0;
            Console.CursorVisible = false;
            while (!ClientTask.IsCompleted)
            {
                var prePos = Console.CursorLeft;//現在カーソル位置を取得
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("　　　　　　　　　　　　　　　　　　");//空白埋め
                Console.CursorLeft = 0;
                Console.Write("接続待受中");
                for (int i = 0; i < cnt % 3; ++i)
                {
                    Console.Write('.');
                }
                cnt++;
                System.Threading.Thread.Sleep(1500);
            }
            Console.WriteLine();
            var client = ClientTask.Result;
            rs.Logger.WriteAndDisplay($"ゲスト（{((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}）と接続しました。");
            Console.CursorVisible = true;

        }

        private static void ClientService()
        {
            using (var client = ClientTask.Result)
            {
                using (NetworkStream ns = client.GetStream())
                {
                    rs.Inject(new Messenger(enc, ns, rs.Logger));
                    //初期通信：相互確認
                    if (rs.Messenger.Recieve() != version)
                    {
                        rs.Logger.WriteAndDisplay("通信相手を信頼することができませんでした。プログラムバージョンに差異はありませんか？");
                        Environment.Exit(1);
                    }
                    else
                    {
                        rs.Messenger.Send(version);
                    }
                    rs.Logger.WriteAndDisplay("信頼できる通信相手を認識しました。");
                    rs.Game.StartLoop(false);
                }
            }
        }
    }
}

