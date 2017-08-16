﻿using System.Text;

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
        static public string version = "1.1.0";
        static public char delimiter = ',';

        static public void outputArrow(string fmt = "") => System.Console.Write($"{fmt}->");
    }
}
