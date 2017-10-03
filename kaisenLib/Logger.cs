using System;
using System.IO;

namespace KaisenLib
{
    /// <summary>
    /// Open()→Write*()→Close()
    /// </summary>
    public class Logger
    {
        private string logDirectory = "log";
        private string logFileName;

        public Logger(string caller)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFileName = logDirectory + Path.DirectorySeparatorChar + caller + DateTime.Now.ToString("_yy_MM_dd_HH_mm_ss_fff") + ".txt";
        }

       ~Logger()
        {
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

        
        private void Logging(DateTime time, string data)
        {
            Logging(MakeLogString(time, data));
        }

        //ファイルに書き込む。
        private void Logging(string data)
        {
            using (var fs = File.AppendText(logFileName))
            {
                    try
                    {
                        fs.WriteLine(data);
                        fs.Flush(); //パフォーマンス問題ないかな？
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("次のデータをファイルに記録できませんでした！: " + data);
                    }
                }            
        }

        private static string MakeLogString(DateTime time, string data)
        {
            return $"[{time}] {data}";
        }
    }
}
