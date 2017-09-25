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
    /// 砲撃応答メッセージ。ToDo:撃破艦船の返送方法を再検討
    /// </summary>
    internal class FiringResponseMsg : SerializableMessage
    {
        internal ResourceSupplier rs;
        internal FiringResponseSummary summary;
        internal string destroyedName;

        private FiringResponseMsg(ResourceSupplier rs)
        {
            this.rs = rs;
            this.MsgId = MessageId.FiringResponse;
            Name = "砲撃";
        }

        internal FiringResponseMsg(FiringResponseSummary summary, ResourceSupplier rs) : this(summary, string.Empty, rs)
        {
        }

        internal FiringResponseMsg(FiringResponseSummary summary, string destroyed, ResourceSupplier rs) : this(rs)
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
