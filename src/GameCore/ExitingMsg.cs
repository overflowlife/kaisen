using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class ExitingMsg : KaisenMsg
    {
        internal ExitingMsg()
        {
            msgId = KaisenMsgId.Exiting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式："{(int)kaisenMsgId.Exiting}"</param>
         internal ExitingMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            int msgId;
            if( !(splited.Length == 1 && int.TryParse(splited[0], out msgId) && (KaisenMsgId)msgId == KaisenMsgId.Exiting))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
            this.msgId = KaisenMsgId.Exiting;
        }

        public override string ToString()
        {
            return $"{(int)msgId}";
        }
    }
}
