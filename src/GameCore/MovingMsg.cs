using KaisenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// <param name="str"></param>
        internal MovingMsg(string str)
        {
            string[] splited = str.Split(AppSet.delimiter);

        }

        internal override string ToString()
        {
            return $"{(int)msgId}{AppSet.delimiter}"
        }
    }
}
