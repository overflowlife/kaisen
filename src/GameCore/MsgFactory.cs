using KaisenLib;
using System;
using static KaisenLib.AppSet;

namespace GameCore
{
    public class MsgFactory
    {
        public static KaisenMsg Manufact(string msg)
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
                case KaisenMsgId.Firing:
                    return new FiringMsg(msg);
                case KaisenMsgId.Moving:
                    return new MovingMsg(msg);
                case KaisenMsgId.Exiting:
                    return new ExitingMsg(msg);
                case KaisenMsgId.None:
                default:
                    throw new ArgumentException("適切なメッセージIDではありません。");
            }
        }
    }
}
