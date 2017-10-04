using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace KaisenLib
{
    /// <summary>
    /// 特定の相手とのTCP通信を行うクラスです。
    /// </summary>
    public class Messenger
    {
        internal Logger logger;

        internal Encoding Enc { get; private set; }
        internal NetworkStream Ns { get; private set; }
        internal MemoryStream Ms { get; private set; }
        internal byte[] recBytes;

        /// <summary>
        /// 新たな<c>Messenger</c>クラスのインスタンスを生成します。通信時に用いるエンコード、NetworkStream、Loggerのインスタンスを与えてください。
        /// </summary>
        /// <param name="enc"></param>
        /// <param name="ns"></param>
        /// <param name="logger"></param>
        public Messenger(Encoding enc, NetworkStream ns, Logger logger)
        {
            this.logger = logger;
            Enc = enc;
            Ns = ns;
            recBytes = new byte[256];
        }

        /// <summary>
        /// 通信相手からのメッセージを同期的に受信します。
        /// </summary>
        /// <returns></returns>
        public string Recieve()
        {
            using (Ms = new MemoryStream())
            {
                var recSize = 0;
                do
                {
                    try
                    {
                        
                        recSize = Ns.Read(recBytes, 0, recBytes.Length);
                    }
                    catch (IOException)
                    {
                        throw;
                    }
                    Ms.Write(recBytes, 0, recSize);
                } while (Ns.DataAvailable || recBytes[recSize - 1] != '\n');

                var recMsg = Enc.GetString(Ms.ToArray());
                recMsg = recMsg.TrimEnd('\n');
                //logger.WriteLine($"受信メッセージ：{recMsg}");
                return recMsg;
            }
        }

        /// <summary>
        /// 通信相手へメッセージを同期的に送信します。
        /// </summary>
        /// <param name="sendMsg"></param>
        public void Send(string sendMsg)
        {
            var sendBytes = Enc.GetBytes(sendMsg + '\n');
            try
            {
                Ns.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (IOException)
            {
                throw;
            }
            //logger.WriteLine($"送信メッセージ：{Enc.GetString(sendBytes)}");

            return;
        }
    }
}
