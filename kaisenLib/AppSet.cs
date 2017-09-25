using System.Text;

namespace KaisenLib
{
    /// <summary>
    /// 共通プロパティを定義します。
    /// </summary>
    public static class AppSet
    {
        /// <summary>
        /// C-S間通信に用いる文字エンコードを提供します。read-onlyです。
        /// </summary>
        static public readonly Encoding enc;
        /// <summary>
        /// アプリケーションで用いる標準ポート番号を提供します。read-onlyです。
        /// </summary>
        static public readonly int defaultPort;
        /// <summary>
        /// <summary>
        /// 現在のアプリケーションバージョン番号を提供します。read-onlyです。
        /// </summary>
        static public readonly string version;
        /// <summary>
        /// 各クラスの（デ）シリアライズメソッドで用いる区切り文字を提供します。read-onlyです。
        /// </summary>
        static public readonly char delimiter;
        /// <summary>
        /// コンソールウィンドウのタイトル文字です。
        /// </summary>
        static public readonly string consoleTitle;

        static AppSet()
        {
            enc = Encoding.UTF8;
            defaultPort = 19690;
            version = "1.1.0";
            delimiter = ',';
            consoleTitle = $"海戦ゲーム ver.{version}";
        }

        static public void OutputArrow(string fmt = string.Empty) => System.Console.Write($"{fmt}->");
    }
}
