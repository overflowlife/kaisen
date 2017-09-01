using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace KaisenLib
{
    public class Messenger
    {
        internal Logger logger;

        internal Encoding Enc { get; private set; }
        internal NetworkStream Ns { get; private set; }
        internal MemoryStream Ms { get; private set; }
        internal byte[] recBytes;

        public Messenger(Encoding enc, NetworkStream ns, Logger logger)
        {
            this.logger = logger;
            Enc = enc;
            Ns = ns;
            recBytes = new byte[256];
        }

        /// <summary>
        /// 
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
                    catch (IOException e)
                    {
                        logger.WriteAndDisplay(e.Message);
                        throw;
                    }
                    Ms.Write(recBytes, 0, recSize);
                } while (Ns.DataAvailable || recBytes[recSize - 1] != '\n');

                var recMsg = Enc.GetString(Ms.ToArray());
                recMsg = recMsg.TrimEnd('\n');
                logger.WriteLine($"受信メッセージ：{recMsg}");
                return recMsg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendMsg"></param>
        public void Send(string sendMsg)
        {
            var sendBytes = Enc.GetBytes(sendMsg + '\n');
            try
            {
                Ns.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (IOException e)
            {
                logger.WriteAndDisplay(e.Message);
                throw;
            }
            logger.WriteLine($"送信メッセージ：{Enc.GetString(sendBytes)}");

            return;
        }

        ~Messenger()
        {
            if (Ns != null)
            {
                Ns.Dispose();
                Ns = null;
            }
            if (Ms != null)
            {
                Ms.Dispose();
                Ms = null;
            }
        }
    }
}
