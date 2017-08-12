using static KaisenLib.AppSet;
using System;

namespace GameCore
{
    internal class FiringRequestMsg : KaisenMsg
    {
        internal int x { get; private set; }
        internal int y { get; private set; }

        internal FiringRequestMsg(int x, int y)
        {
            msgId = KaisenMsgId.FiringRequest;
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式：{(int)msgId}{delimiter}{x}{delimiter}{y}</param>
        internal FiringRequestMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            if ((KaisenMsgId)int.Parse(splited[0]) != KaisenMsgId.FiringRequest || splited.Length != 3)
            {
                throw new ArgumentException("引数チェックでの例外です。");
            }
            msgId = KaisenMsgId.FiringRequest;
            this.x = int.Parse(splited[1]);
            this.y = int.Parse(splited[2]);
        }

        public override string ToString()
        {
            return $"{(int)msgId}{delimiter}{x}{delimiter}{y}";
        }
    }
}
