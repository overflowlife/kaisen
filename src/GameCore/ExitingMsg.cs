using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    public class ExitingMsg : KaisenMsg
    {
        public ExitingMsg()
        {
            msgId = KaisenMsgId.Exiting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式："{(int)kaisenMsgId.Exiting}"</param>
        public ExitingMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            int msgId;
            if( !(splited.Length == 0 && int.TryParse(splited[0], out msgId) && (KaisenMsgId)msgId == KaisenMsgId.Exiting))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
            this.msgId = KaisenMsgId.Exiting;
        }

        internal override string ToString()
        {
            return $"{(int)msgId}";
        }
    }
}
