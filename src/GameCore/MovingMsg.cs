using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    public class MovingMsg : KaisenMsg
    {
        int x;
        int y;
        string sType;

        internal MovingMsg(int x, int y, Ship shooter)
        {
            msgId = KaisenMsgId.Moving;
            this.x = x;
            this.y = y;
            this.sType = shooter.Stype;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        internal MovingMsg(string str)
        {

        }

        internal override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
