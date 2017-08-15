using static KaisenLib.AppSet;
using System;
using System.Diagnostics;

namespace GameCore
{
    internal class MovingRequestMsg : KaisenMsg
    {
        internal int direction;
        internal int distance;
        internal string mover;

        internal MovingRequestMsg(int direction, int distance, string mover)
        {
            msgId = KaisenMsgId.MovingRequest;
            this.direction = direction;
            this.distance = distance;
            this.mover = mover;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">形式：{(int)msgId}{delimiter}{direction}{delimiter}{distance}{delimiter}{mover}</param>
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
            mover = splited[3];
        }

        public override string ToString()
        {
            Debug.Assert(msgId == KaisenMsgId.MovingRequest);
            return $"{(int)msgId}{delimiter}{direction}{delimiter}{distance}{delimiter}{mover}";
        }
    }
}
