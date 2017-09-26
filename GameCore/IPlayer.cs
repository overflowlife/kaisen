using System;
using System.Diagnostics;
using System.Collections.Generic;
namespace GameCore
{
    public abstract class IPlayer
    {
        internal ResourceSupplier rs;
        internal Action<string> logging;
        string Name { get; set; }
        /// <summary>
        /// 艦船を配置し、配置したあとの座標リストを返却します。
        /// </summary>
        /// <returns></returns>
        public abstract List<Point> DeployShips();
        /// <summary>
        /// こちら側のコマンドを発信し、相手側の応答を受け取ります。
        /// </summary>
        /// <returns>終了フラグ</returns>
        public abstract bool DoTurn();
        /// <summary>
        /// 相手側のコマンドを受け取り、処理し、こちら側の応答を発信します。
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>終了フラグ</returns>
        public bool DoResponse()
        {
            string msg = rs.Messenger.Recieve();
            SerializableMessage recieved = MessageFactory.Manufact(msg);
            switch (recieved.MsgId)
            {
                case MessageId.None:
                    break;
                case MessageId.FiringRequest:
                    FiringResponse(recieved as FiringRequestMsg, logging);
                    break;

                case MessageId.MovingRequest:
                    MovingResponse(recieved as MovingRequestMsg, logging);
                    break;

                case MessageId.ExitingRequest:
                    ExitingResponse(recieved as ExitingRequestMsg, logging);
                    break;

                case MessageId.FiringResponse:
                case MessageId.MovingResponse:
                case MessageId.ExitingResponse:
                default:
                    break;
            }

            return recieved.MsgId == MessageId.ExitingRequest;
        }

        internal void FiringResponse(FiringRequestMsg msg, Action<string> logging)
        {
            logging?.Invoke($"地点({msg.x}, {msg.y})が砲撃されました。");
            Debug.Assert(rs.Game.ValidateX(msg.x) && rs.Game.ValidateY(msg.y));
            var send = rs.Game.ShootFromOther(msg.x, msg.y, out Ship hit);
            rs.Messenger.Send(send.ToString());
            switch (send.summary)
            {
                case FiringResponseSummary.Hit:
                    if (send.destroyedName != string.Empty)
                    {
                        logging?.Invoke($"{send.destroyedName}が撃沈されました..");
                    }
                    else
                    {
                        logging?.Invoke($"秘匿情報：{hit.Type}に命中しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    logging?.Invoke("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    logging?.Invoke("海に落ちました。");
                    break;
                default:
                    break;
            }
        }

        internal void MovingResponse(MovingRequestMsg msg, Action<string> logging)
        {
            rs.Logger.WriteAndDisplay($"{msg.mover}が{msg.direction}方向に{msg.distance}移動しました。");
            rs.Messenger.Send(new MovingResponseMsg().ToString());
            rs.Logger.WriteAndDisplay($"移動に対して応答しました。");
        }


        internal void ExitingResponse(ExitingRequestMsg msg, Action<string> logging)
        {
            logging?.Invoke("終了通知を受け取りました。");
            rs.Messenger.Send(new ExitingResponseMsg().ToString());
            logging?.Invoke("終了応答を送信しました。");
        }
    }
}