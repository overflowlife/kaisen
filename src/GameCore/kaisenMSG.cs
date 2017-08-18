namespace GameCore
{
    /// <summary>
    /// ソケットを介してC-S間でやり取りされるメッセージの基底クラスです。
    /// </summary>
    internal abstract class KaisenMsg
    {
        internal KaisenMsgId msgId { get; set; }
        internal string Name { get; set; }
        public override abstract string ToString();
    }


    internal enum KaisenMsgId
    {
        None = 0,
        /// <summary>
        /// 砲撃
        /// </summary>
        FiringRequest = 1,
        /// <summary>
        /// 砲撃応答
        /// </summary>
        FiringResponse = 2,
        /// <summary>
        /// 移動
        /// </summary>
        MovingRequest = 3,
        /// <summary>
        /// 移動応答
        /// </summary>
        MovingResponse = 4,
        /// <summary>
        /// 撤退
        /// </summary>
        ExitingRequest = 8,
        /// <summary>
        /// 撤退応答
        /// </summary>
        ExitingResponse = 9,
    }
}
