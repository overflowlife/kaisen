using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    /// <summary>
    /// ゲーム終了を相手に通知するメッセージです。
    /// </summary>
    internal class ExitingRequestMsg : KaisenMsg
    {
        internal ExitingRequestMsg()
        {
            msgId = KaisenMsgId.ExitingRequest;
            Name = "終了";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式："{(int)kaisenMsgId.Exiting}"</param>
         internal ExitingRequestMsg(string msg) : this()
        {
            string[] splited = msg.Split(delimiter);
            int msgId;
            if( !(splited.Length == 1 && int.TryParse(splited[0], out msgId) && (KaisenMsgId)msgId == KaisenMsgId.ExitingRequest))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
        }

        public override string ToString()
        {
            Debug.Assert(msgId == KaisenMsgId.ExitingRequest);
            return $"{(int)msgId}";
        }
    }
}
