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

            var cts = new CancellationTokenSource();
            ListenWorker listenWorker = new ListenWorker(cts.Token);
            listenWorker.DoWork();
        } 
    }

    internal class ListenWorker
    {
        CancellationToken ct;

        internal ListenWorker(CancellationToken ct)
        {
            this.ct = ct;
        }

        async internal Task DoWork()
        {
            var tcpListener = new TcpListener(IPAddress.Any, defaultPort);
            tcpListener.Start();

            try
            {
                while (true)
                {
                    using (var tcpClient = await tcpListener.AcceptTcpClientAsync())
                    {
                        Console.WriteLine($"{tcpClient.Client.RemoteEndPoint}を受け入れました。");
                        await Task.Run(
                            () =>
                            {
                                Service(tcpClient);
                            });
                    }
                }
            }
            finally
            {
                Console.WriteLine("Listen cancelled.");
                tcpListener.Stop();
            }
        }

        internal void Service(TcpClient tcpClient)
        {
            Messenger.Open(AppSet.enc, tcpClient.GetStream());
            Logger.Open(nameof(AutoHost01));
            Game.RegisterPlayer(new AutomaticPlayer());
            Game.DeployShips();

            //初期通信：相互確認
            if (Messenger.Recieve() != version)
            {
                Logger.WriteAndDisplay("通信相手を信頼することができませんでした。プログラムバージョンに差異はありませんか？");
                Environment.Exit(1);
            }
            else
            {
                Messenger.Send(version);
            }
            Logger.WriteAndDisplay("信頼できる通信相手を認識しました。");
            Game.StartLoop(false);

            Console.WriteLine(Messenger.Recieve());
            Messenger.Send(version);
            Messenger.Close();
            Logger.Close();
        }
    }
}
