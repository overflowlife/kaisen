using System;
using System.IO;

namespace KaisenLib
{
    /// <summary>
    /// ログファイルへの出力を行うクラスです。
    /// </summary>
    public class Logger
    {
        private string logDirectory = "log";
        private string logFileName;

        /// <summary>
        /// 新しい<c>Logger</c>クラスのインスタンスを作成します。引数にはアプリケーション名（ログファイルのプリフィクスになります）を与えてください。
        /// </summary>
        /// <param name="caller"></param>
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

        /// <summary>
        /// ログファイルへの追加書き込みを行います（記録時刻を指示できます）。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        public void WriteLine(DateTime time, string data)
        {
            Logging(time, data);
        }
        /// <summary>
        /// ログファイルへの追加書き込み、およびコンソールへの出力を行います（記録時刻を指示できます）。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        public void WriteAndDisplay(DateTime time, string data)
        {
            var logString = MakeLogString(time, data);
            Console.WriteLine(logString);
            Logging(logString);
        }

        /// <summary>
        /// ログファイルへの追加書き込みを行います。
        /// </summary>
        /// <param name="data"></param>
        public void WriteLine(string data)
        {
            Logging(DateTime.Now, data);
        }

        /// <summary>
        /// ログファイルへの追加書き込みを行います。
        /// </summary>
        /// <param name="data"></param>
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
