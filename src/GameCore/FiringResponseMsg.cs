using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class FiringResponseMsg : KaisenMsg
    {
        internal FiringResponseSummary summary;
        internal string destroyedName;

        private FiringResponseMsg()
        {
            this.msgId = KaisenMsgId.FiringResponse;
            Name = "砲撃";
        }

        internal FiringResponseMsg(FiringResponseSummary summary) : this(summary, Game.Null)
        {
        }

        internal FiringResponseMsg(FiringResponseSummary summary, string destroyed) : this()
        {
            this.summary = summary;
            this.destroyedName = destroyed;
        }

        /// <summary>
        /// 形式：{(int)msgId}{delimiter}{(int)summary}{delimiter}{destroyedName}
        /// </summary>
        /// <param name="msg"></param>
        internal FiringResponseMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            int msgId;
            int summary;
            if (!(splited.Length == 3 && int.TryParse(splited[0], out msgId) && (KaisenMsgId)msgId == KaisenMsgId.FiringResponse)
                || !int.TryParse(splited[1], out summary))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
            this.msgId = KaisenMsgId.FiringResponse;
            this.summary = (FiringResponseSummary)summary;
            this.destroyedName = splited[2];
        }

        public override string ToString()
        {
            Debug.Assert(msgId == KaisenMsgId.FiringResponse);
            return $"{(int)msgId}{delimiter}{(int)summary}{delimiter}{destroyedName}";
        }
    }

    internal enum FiringResponseSummary
    {
        None = 0,
        Hit = 1,
        Nearmiss = 2,
        Water = 3,
    }
}
