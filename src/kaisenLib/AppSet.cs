using System.Text;

namespace KaisenLib
{
    /// <summary>
    /// 共通プロパティを定義します。
    /// </summary>
    public static class AppSet
    {
        static public Encoding enc = Encoding.UTF8;
        static public int defaultPort = 19690;
        static public string loopbackAddress = "127.0.0.1";
        static public string initRequestMsg = "BnEYPXCjA5x93QiHZZb7StXYHt5nxemQV2hnJSpBZpsaXPlzDntSpyDisThx";
        static public string initResponseMsg = "JHfzfNnJFmiWPnZ48bknXMjzEpMHdikNk3rgWSjNY49p9PDST_T";
        static public char delimiter = ',';

        static public void outputArrow(string fmt = "") => System.Console.Write($"{fmt}->");
    }
}
