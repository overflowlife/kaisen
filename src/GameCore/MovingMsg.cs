using static KaisenLib.AppSet;
using System;

namespace GameCore
{
    internal class MovingMsg : KaisenMsg
    {
        int direction;
        int distance;
        string sType;

        internal MovingMsg(int direction, int distance, Ship mover)
        {
            msgId = KaisenMsgId.Moving;
            this.direction = direction;
            this.distance = distance;
            this.sType = mover.Stype;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">形式：{(int)msgId}{delimiter}{direction}{delimiter}{distance}{delimiter}{sType}</param>
        internal MovingMsg(string str)
        {
            string[] splited = str.Split(delimiter);
            if((KaisenMsgId)int.Parse(splited[0]) != KaisenMsgId.Moving || splited.Length != 4)
            {
                throw new ArgumentException("引数チェックでの例外です。");
            }
            direction = int.Parse (splited[1]);
            distance = int.Parse(splited[2]);
            sType = splited[3];
        }

        public override string ToString()
        {
            return $"{(int)msgId}{delimiter}{direction}{delimiter}{distance}{delimiter}{sType}";
        }
    }
}
