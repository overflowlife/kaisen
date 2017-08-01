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
        static public string initRequestMsg = "BnEYPXCjA5x93QiHZZb7StXYHt5nxemQV2hnJSpBZpsaX";
        static public string initResponseMsg = "JHfzfNnJFmiWPnZ48bknXMjzEpMHdikNk3rgWSjNY49p9";
        static public char delimiter = ',';
    }
}
