using KaisenLib;
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
            Dictionary<KaisenMsgId, Action> MsgBinding = new Dictionary<KaisenMsgId, Action>
            {
                { KaisenMsgId.FiringRequest, FiringRequest },
                { KaisenMsgId.MovingRequest, MovingRequest },
                { KaisenMsgId.ExitingRequest, ExitingRequest },
            };
            do
            {
                Console.WriteLine("コマンドを選択してください。");
                foreach (var item in MsgBinding)
                {
                    Console.WriteLine($"{(int)item.Key}: {item.Key}");
                }
                string input = Console.ReadLine();
                validateInput = (int.TryParse(input, out cmd) && MsgBinding.TryGetValue((KaisenMsgId)cmd, out test) );
            } while (!validateInput);

            MsgBinding[(KaisenMsgId)cmd].Invoke();

            return (KaisenMsgId)cmd == KaisenMsgId.ExitingRequest;
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
