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
                try
                {
                    rs = new ResourceSupplier();
                    rs.Inject(new Logger(nameof(AutoHost01)));
                    rs.Inject(new Messenger(enc, tcpClient.GetStream(), rs.Logger));
                    rs.Inject(new Game(rs));
                    rs.Game.RegisterPlayer(new AutomaticPatternRatioShooter(rs));
                    rs.Game.DeployShips();

                    //初期通信：相互確認
                    if (rs.Messenger.Recieve() != version)
                    {
                        throw new Exception("信頼されないクライアントです");
                    }
                    else
                    {
                        rs.Messenger.Send(version);
                    }
                    rs.Logger.WriteLine($"{tcpClient.Client.RemoteEndPoint}を信頼しました。");
                    rs.Game.StartLoop(false);
                }
                catch (Exception e)
                {
                    rs.Logger.WriteAndDisplay($"Exception raised in thread({tcpClient.Client.RemoteEndPoint}). {e.Message} ");
                }
                finally
                {
                    Console.WriteLine($"{tcpClient.Client.RemoteEndPoint}が終了しました。");
                }
                
            }
        }
    }
}
