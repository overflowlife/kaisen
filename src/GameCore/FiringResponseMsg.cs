﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class FiringResponseMsg : SerializableMessage
    {
        internal FiringResponseSummary summary;
        internal string destroyedName;

        private FiringResponseMsg()
        {
            this.MsgId = MessageId.FiringResponse;
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
            if (!(splited.Length == 3 && int.TryParse(splited[0], out int msgId) && (MessageId)msgId == MessageId.FiringResponse)
                || !int.TryParse(splited[1], out int summary))
            {
                throw new ArgumentException("引数チェックの例外です");
            }
            this.MsgId = MessageId.FiringResponse;
            this.summary = (FiringResponseSummary)summary;
            this.destroyedName = splited[2];
        }

        public override string ToString()
        {
            Debug.Assert(MsgId == MessageId.FiringResponse);
            return $"{(int)MsgId}{delimiter}{(int)summary}{delimiter}{destroyedName}";
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
