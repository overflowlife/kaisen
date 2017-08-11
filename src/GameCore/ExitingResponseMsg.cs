using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        }

        public override string ToString()
        {
            return $"{(int)msgId}";
        }
    }
}
