using KaisenLib;
using System;
using static KaisenLib.AppSet;

namespace GameCore
{
    /// <summary>
    /// Manufact(string)によるメッセージファクトリを提供します。
    /// </summary>
    internal class MessageFactory
    {
        internal static SerializableMessage Manufact(string msg)
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
            switch ((MessageId)msgId)
            {
                case MessageId.FiringRequest:
                    return new FiringRequestMsg(msg);
                case MessageId.FiringResponse:
                    return new FiringResponseMsg(msg);
                case MessageId.MovingRequest:
                    return new MovingRequestMsg(msg);
                case MessageId.MovingResponse:
                    return new MovingResponseMsg(msg);
                case MessageId.ExitingRequest:
                    return new ExitingRequestMsg(msg);
                case MessageId.ExitingResponse:
                    return new ExitingResponseMsg(msg);
                case MessageId.None:
                default:
                    throw new ArgumentException($"適切なメッセージIDではありません。:{msgId}");
            }
        }
    }
}
