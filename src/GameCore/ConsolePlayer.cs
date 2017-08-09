using KaisenLib;
using static KaisenLib.AppSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    internal class ConsolePlayer : IPlayer
    {
        public string Name { get; set; }
        internal ConsolePlayer(string name)
        {
            Name = name;
        }

        public bool DoTurn()
        {
            int cmd;
            Action test;
            bool validateInput;
            //発行可能なメッセージの定義と、メッセージを発行するメソッドの対応付け
            Dictionary<int, Action> MsgBinding = new Dictionary<int, Action>
            {//Dictionary.Keyにenumを使うと遅いらしい
                { (int)KaisenMsgId.FiringRequest, FiringRequest },
                { (int)KaisenMsgId.MovingRequest, MovingRequest },
                { (int)KaisenMsgId.ExitingRequest, ExitingRequest },
            };
            do
            {
                Console.WriteLine("コマンドを選択してください。");
                outputArrow();
                foreach (var item in MsgBinding)
                {
                    Console.WriteLine($"{(int)item.Key}: {item.Key}");
                }
                
                string input = Console.ReadLine();
                validateInput = (int.TryParse(input, out cmd) && MsgBinding.TryGetValue(cmd, out test) );
                if (!validateInput)
                {
                    Console.WriteLine("入力に誤りがあります。");
                }
            } while (!validateInput);

            MsgBinding[cmd].Invoke();//信頼済み

            return (KaisenMsgId)cmd == KaisenMsgId.ExitingRequest;//ここ微妙？
        }

        private void ExitingRequest()
        {
            Console.WriteLine(nameof(ExitingRequest));
        }

        private void MovingRequest()
        {
            Console.WriteLine(nameof(MovingRequest));
        }

        private void FiringRequest()
        {
            Console.WriteLine(nameof(FiringRequest));
        }

        public bool Recieve(string msg)
        {
            Console.WriteLine(nameof(Recieve));
            return false;
        }
    }
}
