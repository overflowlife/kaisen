using static KaisenLib.AppSet;
using System;

namespace GameCore
{
    internal class MovingRequestMsg : KaisenMsg
    {
        int direction;
        int distance;
        string sType;

        internal MovingRequestMsg(int direction, int distance, Ship mover)
        {
            msgId = KaisenMsgId.MovingRequest;
            this.direction = direction;
            this.distance = distance;
            this.sType = mover.Stype;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">形式：{(int)msgId}{delimiter}{direction}{delimiter}{distance}{delimiter}{sType}</param>
        internal MovingRequestMsg(string str)
        {
            string[] splited = str.Split(delimiter);
            if((KaisenMsgId)int.Parse(splited[0]) != KaisenMsgId.MovingRequest || splited.Length != 4)
            {
                throw new ArgumentException("引数チェックでの例外です。");
            }
            msgId = KaisenMsgId.MovingRequest;
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
