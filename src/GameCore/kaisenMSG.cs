namespace GameCore
{
    internal abstract class KaisenMsg
    {
        internal KaisenMsgId msgId { get; set; }
        internal string Name { get; set; }
        public override abstract string ToString();
    }

    internal enum KaisenMsgId
    {
        None = 0,
        FiringRequest = 1,
        FiringResponse = 2,
        MovingRequest = 3,
        MovingResponse = 4,
        ExitingRequest = 8,
        ExitingResponse = 9,
    }
}
