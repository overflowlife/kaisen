using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class MovingResponseMsg : SerializableMessage
    {
        internal MovingResponseMsg()
        {
            MsgId = MessageId.MovingResponse;
            Name = "移動";
        }

        internal MovingResponseMsg(string msg) : this()
        {
            string[] splited = msg.Split(delimiter);
            if (!(splited.Length == 1 && int.TryParse(splited[0], out int msgId) && (MessageId)msgId == MessageId.MovingResponse))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
        }

        public override string ToString()
        {
            Debug.Assert(MsgId== MessageId.MovingResponse);
            return $"{(int)MsgId}";
        }
    }
}
