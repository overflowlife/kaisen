using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KaisenLib;
using static KaisenLib.AppSet;
using GameCore;
using System.Text;
using System.Threading.Tasks;

namespace AutoHost01
{
    class Program
    {
        static void Main(string[] args)
        {
            MainWorker().Wait();
        }

        static async Task MainWorker()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("海戦ゲーム：自動ホスト01を起動します。");
            ListenWorker listenWorker = new ListenWorker();
            await listenWorker.DoWork();
        } 
    }

    internal class ListenWorker
    {
        public ResourceSupplier rs { get; internal set; }

        internal ListenWorker()
        {

        }

        async internal Task DoWork()
        {
            var tcpListener = new TcpListener(IPAddress.Any, defaultPort);
            tcpListener.Start();
            try
            {
                while (true)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine($"{tcpClient.Client.RemoteEndPoint}を受け入れました。");
                    Task.Run(
                        () =>
                        {
                            Service(tcpClient);
                        });
                }
            }
            finally
            {
                tcpListener.Stop();
                Console.WriteLine("Listen cancelled.");
            }

        }

        internal void Service(TcpClient tcpClient)
        {
            using (tcpClient)
            {
                rs = new ResourceSupplier();
                rs.Inject(new Messenger(enc, tcpClient.GetStream(), rs.Logger));
                rs.Inject(new Logger(nameof(AutoHost01)));
                rs.Inject(new Game(rs));
                rs.Game.RegisterPlayer(new AutomaticPlayer(rs));
                rs.Game.DeployShips();

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
