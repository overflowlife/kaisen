using System;
using KaisenLib;
using static KaisenLib.AppSet;
using System.Text;

namespace AutoHost01
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Open(nameof(AutoHost01));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Logger.WriteAndDisplay("海戦ゲーム：自動ホスト01を起動します。");


            
        }
    }
}
