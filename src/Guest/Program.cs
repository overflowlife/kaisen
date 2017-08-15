using System;
using System.Net;
using System.Net.Sockets;
using KaisenLib;
using static KaisenLib.AppSet;
using GameCore;

namespace Guest
{
    class Program
    {
        static NetworkStream ns;
        static void Main(string[] args)
        {
            Console.OutputEncoding = enc;
            Logger.Open(nameof(Guest));
            Logger.WriteAndDisplay("海戦ゲーム：ゲストサイドを起動します。");
            Game.DeployShips();

            string input;
            IPAddress remoteAddress = null;
            int remotePort = -1;
            bool validateInput = false;
            do
            {
                Console.WriteLine($"接続先サーバアドレス、ポート番号を入力して下さい（{loopbackAddress}:{defaultPort}）。");
                outputArrow();
                input = Console.ReadLine();
                var addressandPort = input.Split(':');
                if (addressandPort.Length != 2)
                {
                    if (input == "default")
                    {
                        remoteAddress = IPAddress.Parse(loopbackAddress);
                        remotePort = defaultPort;
                        validateInput = true;
                    }
                    else
                    {
                        Console.WriteLine("入力形式が間違っています。");
                    }
                    continue;
                }

                validateInput = IPAddress.TryParse(addressandPort[0], out remoteAddress);
                validateInput &= int.TryParse(addressandPort[1], out remotePort) && (IPEndPoint.MinPort <= remotePort && remotePort <= IPEndPoint.MaxPort);
                if (!validateInput)
                {
                    Console.WriteLine("入力情報が正しくないです。");
                }
            } while (!validateInput);

            using (var tcpClient = new TcpClient(AddressFamily.InterNetwork))
            {
                try
                {
                    tcpClient.ConnectAsync(remoteAddress, remotePort).Wait();
                }
                catch (Exception)
                {
                    Console.WriteLine("接続確立に失敗しました。終了します。");
                    Environment.Exit(1);
                }

                Logger.WriteAndDisplay($"ホスト({((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}:{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port})と接続しました" +
                    $"({((IPEndPoint)tcpClient.Client.LocalEndPoint).Address}:{((IPEndPoint)tcpClient.Client.LocalEndPoint).Port})。");
                using (ns = tcpClient.GetStream())
                {
                    Messenger.Open(enc, ns);
                    //初期通信：相互確認
                    Messenger.Send(version);
                    if (Messenger.Recieve() != version)
                    {
                        Logger.WriteAndDisplay("通信相手を信頼することができませんでした。プログラムバージョンに差異はありませんか？");
                        Environment.Exit(1);
                    }
                    Logger.WriteAndDisplay("信頼できる通信相手を認識しました。");
                    Game.StartLoop(true);
                    Messenger.Close();
                }
            }

            Logger.WriteAndDisplay("海戦ゲーム：ゲストサイドを終了します。");
            Logger.Close();
        }
    }
}