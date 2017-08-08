using System;
using System.IO;

namespace KaisenLib
{
    public class Logger : IDisposable
    {
        private StreamWriter sw;
        private string logDirectory = "log";
        private string logFileName;

        public Logger(string caller)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFileName = logDirectory + Path.DirectorySeparatorChar + caller + DateTime.Now.ToString("_yy_MM_dd_HH_mm_ss_fff") + ".txt";
            var fs = File.Create(logFileName);

            sw = new StreamWriter(fs);

        }

        /// <summary>
        /// This is  vey important!!
        /// </summary>
        public void Dispose()
        {
            if (sw != null)
            {
                sw.Dispose();
                sw = null;
            }
        }

        public void WriteLine(DateTime time, string data)
        {
            Logging(time, data);
        }
        public void WriteAndDisplay(DateTime time, string data)
        {
            var logString = MakeLogString(time, data);
            Console.WriteLine(logString);
            Logging(logString);
        }

        public void WriteLine(string data)
        {
            Logging(DateTime.Now, data);
        }
        public void WriteAndDisplay(string data)
        {
            var logString = MakeLogString(DateTime.Now, data);
            Console.WriteLine(logString);
            Logging(logString);
        }

        //ファイルに書き込む。
        private void Logging(DateTime time, string data)
        {
            try
            {
                sw.WriteLine(MakeLogString(time, data));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Logging(string data)
        {
            try
            {
                sw.WriteLine(data);
            }
            catch (Exception)
            {
                Console.WriteLine("次のログをファイルに記録できませんでした！: " + data);
            }
        }

        private string MakeLogString(DateTime time, string data)
        {
            return $"[{time}, {data}]";
        }
    }
}
