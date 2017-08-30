using static KaisenLib.AppSet;
using System;
using System.Diagnostics;

namespace GameCore
{
    /// <summary>
    /// ある地点への砲撃を相手に通知するメッセージ形式です。
    /// </summary>
    internal class FiringRequestMsg : SerializableMessage
    {
        internal int x { get; private set; }
        internal int y { get; private set; }

        private FiringRequestMsg()
        {
            MsgId = MessageId.FiringRequest;
            Name = "砲撃";
        }

        internal FiringRequestMsg(int x, int y) :this()
        {            
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式：{(int)msgId}{delimiter}{x}{delimiter}{y}</param>
        internal FiringRequestMsg(string msg) :this()
        {
            string[] splited = msg.Split(delimiter);
            if ((MessageId)int.Parse(splited[0]) != MessageId.FiringRequest || splited.Length != 3)
            {
                throw new ArgumentException("引数チェックでの例外です。");
            }
            this.x = int.Parse(splited[1]);
            this.y = int.Parse(splited[2]);
        }

        public override string ToString()
        {
            Debug.Assert(MsgId == MessageId.FiringRequest);
            return $"{(int)MsgId}{delimiter}{x}{delimiter}{y}";
        }
    }
}
