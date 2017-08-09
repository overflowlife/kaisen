using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace KaisenLib
{
    public static class Messenger
    {
        internal static bool isOpen { get; private set; }
        internal static Encoding Enc { get; private set; }
        internal static NetworkStream Ns { get; private set; }
        internal static MemoryStream Ms { get; private set; }
        public static bool disconnected;
        public static byte[] recBytes;

        public static void Open(Encoding enc, NetworkStream ns)
        {
            if (!isOpen)
            {
                isOpen = true;
                Enc = enc;
                Ns = ns;
                disconnected = false;
                recBytes = new byte[256];
            }
        }

        public static string Recieve()
        {
            using (Ms = new MemoryStream())
            {
                var recSize = 0;
                do
                {
                    recSize = Ns.Read(recBytes, 0, recBytes.Length);
                    if (recSize == 0)
                    {
                        disconnected = true;
                        Logger.WriteAndDisplay("相手が切断しました。");
                        break;
                    }
                    Ms.Write(recBytes, 0, recSize);
                } while (Ns.DataAvailable || recBytes[recSize - 1] != '\n');

                var recMsg = Enc.GetString(Ms.ToArray());
                recMsg = recMsg.TrimEnd('\n');
                Logger.WriteLine($"受信メッセージ：{recMsg}");
                return recMsg;
            }

        }

        public static void Send(string sendMsg)
        {
            try
            {
                if (!disconnected)
                {
                    var sendBytes = Enc.GetBytes(sendMsg + '\n');
                    Ns.Write(sendBytes, 0, sendBytes.Length);
                    Logger.WriteLine($"送信メッセージ：{sendMsg}");
                }
            }
            catch (Exception)
            {

                throw;
            }

            return;
        }

        /// <summary>
        /// This is very important
        /// </summary>
        public static void Close()
        {
            if (isOpen)
            {
                isOpen = false;
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
}
