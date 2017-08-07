﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using KaisenLib;

namespace Host
{
    class Program
    {
        static TcpListener listener;
        static Task<TcpClient> clientTask;

        public static Task<TcpClient> ClientTask;
        public static TcpListener Listener;

        static void Main(string[] args)
        {
            Console.OutputEncoding = AppSet.enc;
            Logger.Open(nameof(Host));
            Logger.WriteAndDisplay("海戦ゲーム：ホストサイドを起動します。");

            string input;
            int listenPort;
            var resultParse = false;
            do
            {
                Console.Write("待ち受けポート番号（1～65535）を入力してください。デフォルト値を使用する場合には0を入力してください。\n->");
                input = Console.ReadLine();
                //入力が0ならデフォルト番号を使用
                resultParse = int.TryParse(input, out listenPort);
                if (resultParse && listenPort == 0)
                {
                    listenPort = AppSet.defaultPort;
                    resultParse = true;
                }
                else
                {
                    resultParse = resultParse && (IPEndPoint.MinPort <= listenPort && listenPort <= IPEndPoint.MaxPort);
                }

            } while (!resultParse);

            ListenHelper listenHelper;
            listenHelper = new ListenHelper(listenPort);
                try
                {
                    try
                    {
                        Listener = listenHelper.Listener;
                        Listener.Start();
                        Logger.WriteAndDisplay($"ポート番号[{listenPort}]を使用して接続要求待ち受けを開始します。");
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
            
            Logger.WriteAndDisplay("海戦ゲーム：ホストサイドを終了します。");
            Logger.Close();
            return;
        }

        private async static void WaitForAccept()
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
                cnt++;
                Console.Write("接続待受中");
                for (int i = 0; i < cnt % 3; ++i)
                {
                    Console.Write('.');
                }
                System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine();
            var client = ClientTask.Result;
            Logger.WriteAndDisplay($"{((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}と接続しました。");
            Console.CursorVisible = true;

        }

        private static void ClientService()
        {
            using (var client = ClientTask.Result)
            {
                using (NetworkStream ns = client.GetStream())
                {
                    var enc = AppSet.enc;
                    var messenger = new Messenger(enc, ns);

                    //初期通信：相互確認
                    if (messenger.Recieve() != AppSet.initRequestMsg)
                    {
                        Environment.Exit(1);
                    }
                    else
                    {
                        messenger.Send(AppSet.initResponseMsg);
                    }
                    Logger.WriteAndDisplay("信頼できる通信相手を認識しました。");
                }
            }
        }
    }


}

