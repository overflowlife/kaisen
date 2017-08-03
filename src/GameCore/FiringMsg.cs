using static KaisenLib.AppSet;
using System;

namespace GameCore
{
    internal class FiringMsg : KaisenMsg
    {
        int x;
        int y;
        string sType;

        internal FiringMsg(int x, int y, Ship shooter)

        {
            msgId = KaisenMsgId.Firing;
            this.x = x;
            this.y = y;
            this.sType = shooter.Stype;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">形式：{msgId}{AppSet.delimiter}{x}{AppSet.delimiter}{y}{AppSet.delimiter}{sType}</param>
        internal FiringMsg(string msg)
        {
            string[] splited = msg.Split(delimiter);
            if ((KaisenMsgId)int.Parse(splited[0]) != KaisenMsgId.Firing || splited.Length != 4)
            {
                throw new ArgumentException();
            }
            this.x = int.Parse(splited[1]);
            this.y = int.Parse(splited[2]);
            this.sType = splited[3];
        }

        internal override string ToString()
        {
            return $"{(int)msgId}{delimiter}{x}{delimiter}{y}{delimiter}{sType}";
        }
    }
}
