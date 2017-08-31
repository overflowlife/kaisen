using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace KaisenLib
{
    public static class Messenger
    {
        internal static bool IsOpen { get; private set; }
        internal static Encoding Enc { get; private set; }
        internal static NetworkStream Ns { get; private set; }
        internal static MemoryStream Ms { get; private set; }
        internal static byte[] recBytes;

        public static void Open(Encoding enc, NetworkStream ns)
        {
            if (!IsOpen)
            {
                IsOpen = true;
                Enc = enc;
                Ns = ns;
                recBytes = new byte[256];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Recieve()
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
                        Logger.WriteAndDisplay(e.Message);
                        Environment.Exit(1);
                    }
                    Ms.Write(recBytes, 0, recSize);
                } while (Ns.DataAvailable || recBytes[recSize - 1] != '\n');

                var recMsg = Enc.GetString(Ms.ToArray());
                recMsg = recMsg.TrimEnd('\n');
                Logger.WriteLine($"受信メッセージ：{recMsg}");
                return recMsg;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendMsg"></param>
        public static void Send(string sendMsg)
        {
            var sendBytes = Enc.GetBytes(sendMsg + '\n');
            try
            {
                Ns.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (IOException e)
            {
                Logger.WriteAndDisplay(e.Message);
                Environment.Exit(1);
            }
            Logger.WriteLine($"送信メッセージ：{sendMsg}");

            return;
        }

        /// <summary>
        /// This is very important
        /// </summary>
        public static void Close()
        {
            if (IsOpen)
            {
                IsOpen = false;
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
