namespace GameCore
{
    internal abstract class KaisenMsg
    {
        internal KaisenMsgId msgId { get; set; }
        internal new abstract string ToString();
    }

    internal enum KaisenMsgId
    {
        None = 0,
        Firing = 1,
        Moving = 2,
        Exiting = 9,
    }
}
