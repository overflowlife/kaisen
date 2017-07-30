using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace KaisenLib
{
    public class Messenger
    {
        public Encoding Enc { get; private set; }
        private NetworkStream ns;
        private MemoryStream ms;
        private bool disconnected;
        private byte[] recBytes;

        public Messenger()
        {
            Enc = null;
            ns = null;
            ms = null;
            disconnected = false;
            recBytes = new byte[256];
        }

        public Messenger(Encoding enc, NetworkStream ns)
        {
            this.Enc = enc;
            this.ns = ns;
            disconnected = false;
            recBytes = new byte[256];
        }

        ~Messenger()
        {
            if (ns != null)
            {
                ns.Dispose();
                ns = null;
            }
            if (ms != null)
            {
                ms.Dispose();
                ms = null;
            }
        }

        public string Recieve()
        {
            using (ms = new MemoryStream())
            {
                var recSize = 0;
                do
                {
                    recSize = ns.Read(recBytes, 0, recBytes.Length);
                    if (recSize == 0)
                    {
                        disconnected = true;
                        Logger.WriteAndDisplay("相手が切断しました。");
                        break;
                    }
                    ms.Write(recBytes, 0, recSize);
                } while (ns.DataAvailable || recBytes[recSize - 1] != '\n');

                var recMsg = Enc.GetString(ms.ToArray());
                recMsg = recMsg.TrimEnd('\n');
                Logger.WriteLine($"受信メッセージ：{recMsg}");
                return recMsg;
            }

        }

        public void Send(string sendMsg)
        {
            try
            {
                if (!disconnected)
                {
                    var sendBytes = Enc.GetBytes(sendMsg + '\n');
                    ns.Write(sendBytes, 0, sendBytes.Length);
                    Logger.WriteLine($"送信メッセージ：{sendMsg}");
                }
            }
            catch (Exception)
            {

                throw;
            }

            return;
        }
    }
}
