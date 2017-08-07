namespace GameCore
{
    public abstract class KaisenMsg
    {
        internal KaisenMsgId msgId { get; set; }
        public override abstract string ToString();
    }

    internal enum KaisenMsgId
    {
        None = 0,
        Firing = 1,
        Moving = 2,
        Exiting = 9,
    }
}
