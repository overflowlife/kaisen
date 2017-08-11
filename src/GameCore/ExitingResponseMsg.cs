using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class ExitingResponseMsg : KaisenMsg
    {
        internal ExitingResponseMsg()
        {
            msgId = KaisenMsgId.ExitingResponse;
        }

        internal ExitingResponseMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            int msgId;
            if (!(splited.Length == 1 && int.TryParse(splited[0], out msgId) && (KaisenMsgId)msgId == KaisenMsgId.ExitingResponse))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
            this.msgId = KaisenMsgId.ExitingResponse;
        }

        public override string ToString()
        {
            return $"{(int)msgId}";
        }
    }
}
