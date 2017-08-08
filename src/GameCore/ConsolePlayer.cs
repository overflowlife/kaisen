using KaisenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    public class ConsolePlayer : IPlayer
    {
        internal Messenger messenger;
        internal Logger logger;

        internal ConsolePlayer(Messenger messenger, Logger logger)
        {
            this.messenger = messenger;
            this.logger = logger;
        }

        public bool DoTurn()
        {
            int cmd;
            bool validateInput;
            do
            {
                Console.Write("コマンドを選択してください。\n1: Fire..\n3: Move..\n8: Exit..\n->");
                string input = Console.ReadLine();
                validateInput = (int.TryParse(input, out cmd) && (KaisenMsgId)cmd == KaisenMsgId.FiringRequest || (KaisenMsgId)cmd == KaisenMsgId.MovingRequest || (KaisenMsgId)cmd == KaisenMsgId.ExitingRequest);
            } while (!validateInput);

            switch ((KaisenMsgId)cmd)
                {
                    case KaisenMsgId.FiringRequest:
                        FiringRequest();
                        break;
                    case KaisenMsgId.MovingRequest:
                        MovingRequest();
                        break;
                    case KaisenMsgId.ExitingRequest:
                        ExitingRequest();
                        break;
                    case KaisenMsgId.None:
                    case KaisenMsgId.FiringResponse:
                    case KaisenMsgId.MovingResponse:
                    case KaisenMsgId.ExitingResponse:
                    default:
                        throw new Exception("異常発生");
                }

            return false;
        }

        private void ExitingRequest()
        {
            throw new NotImplementedException();
        }

        private void MovingRequest()
        {
            throw new NotImplementedException();
        }

        private void FiringRequest()
        {
            throw new NotImplementedException();
        }

        public bool Recieve(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
