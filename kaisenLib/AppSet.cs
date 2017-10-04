using System.Text;

namespace KaisenLib
{
    /// <summary>
    /// 共通プロパティを定義します。
    /// </summary>
    public static class AppSet
    {
        /// <summary>
        /// C-S間通信に用いる文字エンコードを提供します。読み取り専用です。
        /// </summary>
        static public readonly Encoding enc;
        /// <summary>
        /// アプリケーションで用いる標準ポート番号を提供します。読み取り専用です。
        /// </summary>
        static public readonly int defaultPort;
        /// <summary>
        /// <summary>
        /// 現在のアプリケーションバージョン番号を提供します。読み取り専用です。
        /// </summary>
        static public readonly string version;
        /// <summary>
        /// 各クラスの（デ）シリアライズメソッドで用いる区切り文字を提供します。読み取り専用です。
        /// </summary>
        static public readonly char delimiter;
        /// <summary>
        /// コンソールウィンドウのタイトル文字です。読み取り専用です。
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

        /// <summary>
        /// 指定した文字列の案内を含む、ユーザに入力を促す記号を出力します。
        /// </summary>
        /// <param name="fmt"></param>
        static public void OutputArrow(string fmt = "") => System.Console.Write($"{fmt}->");
    }
}
