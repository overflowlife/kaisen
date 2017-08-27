using System;
using System.IO;

namespace KaisenLib
{
    /// <summary>
    /// Open()→Write*()→Close()
    /// </summary>
    public static class Logger
    {
        internal static bool IsOpened { get; set; }
        private static StreamWriter sw;
        private static string logDirectory = "log";
        private static string logFileName;

        public static void Open(string caller)
        {
            if (!IsOpened)
            {
                IsOpened = true;
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                logFileName = logDirectory + Path.DirectorySeparatorChar + caller + DateTime.Now.ToString("_yy_MM_dd_HH_mm_ss_fff") + ".txt";
                var fs = File.Create(logFileName);

                sw = new StreamWriter(fs);
            }
        }

        /// <summary>
        /// This is  vey important!!
        /// </summary>
        public static void Close()
        {
            if (IsOpened)
            {
                IsOpened = false;
                if (sw != null)
                {
                    sw.Dispose();
                    sw = null;
                }
            }

        }

        public static void WriteLine(DateTime time, string data)
        {
            Logging(time, data);
        }
        public static void WriteAndDisplay(DateTime time, string data)
        {
            var logString = MakeLogString(time, data);
            Console.WriteLine(logString);
            Logging(logString);
        }

        public static void WriteLine(string data)
        {
            Logging(DateTime.Now, data);
        }

        public static void WriteAndDisplay(string data)
        {
            var logString = MakeLogString(DateTime.Now, data);
            Console.WriteLine(logString);
            Logging(logString);
        }

        
        private static void Logging(DateTime time, string data)
        {
            Logging(MakeLogString(time, data));
        }

        //ファイルに書き込む。
        private static void Logging(string data)
        {
            try
            {
                sw.WriteLine(data);
                sw.Flush(); //パフォーマンス問題ないかな？
            }
            catch (Exception)
            {
                Console.WriteLine("次のデータをファイルに記録できませんでした！: " + data);
            }
        }

        private static string MakeLogString(DateTime time, string data)
        {
            return $"[{time}] {data}";
        }
    }
}
