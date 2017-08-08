using static KaisenLib.AppSet;
using System;

namespace GameCore
{
    internal class FiringRequestMsg : KaisenMsg
    {
        int x;
        int y;
        string sType;

        internal FiringRequestMsg(int x, int y, Ship shooter)
        {
            msgId = KaisenMsgId.FiringRequest;
            this.x = x;
            this.y = y;
            this.sType = shooter.Stype;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式：{(int)msgId}{delimiter}{x}{delimiter}{y}{delimiter}{sType}</param>
        internal FiringRequestMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            if ((KaisenMsgId)int.Parse(splited[0]) != KaisenMsgId.FiringRequest || splited.Length != 4)
            {
                throw new ArgumentException("引数チェックでの例外です。");
            }
            this.x = int.Parse(splited[1]);
            this.y = int.Parse(splited[2]);
            this.sType = splited[3];
        }

        public override string ToString()
        {
            return $"{(int)msgId}{delimiter}{x}{delimiter}{y}{delimiter}{sType}";
        }
    }
}
