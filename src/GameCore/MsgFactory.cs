using KaisenLib;
using System;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class MsgFactory
    {
        internal static KaisenMsg Manufact(string msg)
        {
            int msgId;
            string[] splited = msg.Split(delimiter);
            if(splited.Length == 0)
            {
                throw new ArgumentException("要素がありません");
            }
            if(!int.TryParse(splited[0], out msgId))
            {
                throw new ArgumentException("第一要素が数値ではありません。");
            }
            switch ((KaisenMsgId)msgId)
            {
                case KaisenMsgId.FiringRequest:
                    return new FiringRequestMsg(msg);
                /*case KaisenMsgId.FiringResponse:
                    return new FiringResponseMsg(msg);*/
                case KaisenMsgId.MovingRequest:
                    return new MovingRequestMsg(msg);
                /*case KaisenMsgId.MovingResponse:
                    return new MovingResponseMsg(msg);*/
                case KaisenMsgId.ExitingRequest:
                    return new ExitingRequestMsg(msg);
                case KaisenMsgId.ExitingResponse:
                    return new ExitingResponseMsg(msg);
                case KaisenMsgId.None:
                default:
                    throw new ArgumentException("適切なメッセージIDではありません。");
            }
        }
    }
}
