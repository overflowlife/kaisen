using System;
using System.Text;
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
        public static ResourceSupplier rs;
        static void Main(string[] args)
        {
            Console.Title = consoleTitle;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            rs = new ResourceSupplier();
            rs.Inject(new Logger(nameof(Guest)));
            rs.Logger.WriteAndDisplay("海戦ゲーム：ゲストサイドを起動します。");
            rs.Inject(new Game(rs));
            rs.Game.RegisterPlayer(new ConsolePlayer("You", rs));
            rs.Game.DeployShips();

            string input;
            IPAddress remoteAddress = null;
            int remotePort = -1;
            bool validateInput = false;
            do
            {
                Console.WriteLine($"接続先サーバアドレス、ポート番号を入力して下さい（{IPAddress.Loopback}:{defaultPort}）。");
                OutputArrow();
                input = Console.ReadLine();
                var addressandPort = input.Split(':');
                if (addressandPort.Length != 2)
                {
                    if (input == "default")
                    {
                        remoteAddress = IPAddress.Loopback;
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

                rs.Logger.WriteAndDisplay($"ホスト（{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}:{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}）と接続しました" +
                    $"({((IPEndPoint)tcpClient.Client.LocalEndPoint).Address}:{((IPEndPoint)tcpClient.Client.LocalEndPoint).Port})。");
                using (ns = tcpClient.GetStream())
                {
                    rs.Inject(new Messenger(enc, ns, rs.Logger));
                    //初期通信：相互確認
                    rs.Messenger.Send(version);
                    if (rs.Messenger.Recieve() != version)
                    {
                        rs.Logger.WriteAndDisplay("通信相手を信頼することができませんでした。プログラムバージョンに差異はありませんか？");
                        Environment.Exit(1);
                    }
                    rs.Logger.WriteAndDisplay("信頼できる通信相手を認識しました。");
                    rs.Game.StartLoop(true);
                }
            }

            rs.Logger.WriteAndDisplay("海戦ゲーム：ゲストサイドを終了します。");
        }
    }
}