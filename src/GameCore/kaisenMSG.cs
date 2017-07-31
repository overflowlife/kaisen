using System;

namespace GameCore
{
    internal abstract class KaisenMsg
    {
        internal KaisenMsgId msgId { get; set; }

        internal abstract void FromString(string msg);
        internal new abstract string ToString();
    }

    internal enum KaisenMsgId
    {
        None = 0,
        Firing = 1,
        Moving = 2,
        Exiting = 9,
    }

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

        internal override void FromString(string msg)
        {
            throw new NotImplementedException();
        }

        internal override string ToString()
        {
            return "";
        }
    }
}
