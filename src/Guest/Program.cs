using System;
using System.Net;
using System.Net.Sockets;
using KaisenLib;
using GameCore;

namespace Guest
{
    class Program
    {
        static NetworkStream ns;
        static void Main(string[] args)
        {
            Console.OutputEncoding = AppSet.enc;
            Logger.Open(nameof(Guest));
            Logger.WriteAndDisplay("海戦ゲーム：ゲストサイドを起動します。");
            string input;
            IPAddress remoteAddress = null;
            var remotePort = -1;
            var validateInput = false;
            do
            {
                Console.Write("接続先サーバアドレス、ポート番号を入力して下さい（127.0.0.1:19690）。\n->");
                input = Console.ReadLine();
                var addressandPort = input.Split(':');
                if (addressandPort.Length != 2)
                {
                    Console.WriteLine("入力形式が間違っています。");
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

                    throw;
                }

                Logger.WriteAndDisplay($"ホスト({((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}:{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port})と接続しました" +
                    $"({((IPEndPoint)tcpClient.Client.LocalEndPoint).Address}:{((IPEndPoint)tcpClient.Client.LocalEndPoint).Port})。");
                using (ns = tcpClient.GetStream())
                {
                    var enc = AppSet.enc;
                    var messenger = new Messenger(enc, ns);

                    //初期通信：相互確認
                    messenger.Send(AppSet.initRequestMsg);
                    if (messenger.Recieve() != AppSet.initResponseMsg)
                    {
                        Environment.Exit(1);
                    }
                    Logger.WriteAndDisplay("信頼できる通信相手を認識しました。");
                }
            }

            Logger.WriteAndDisplay("海戦ゲーム：ゲストサイドを終了します。");
            Logger.Close();
        }
    }
}