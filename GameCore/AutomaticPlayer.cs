using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaisenLib;
using static KaisenLib.AppSet;
using System.Diagnostics;

namespace GameCore
{
    /// <summary>
    /// 自動応答するプレイヤのプロトタイプです。
    /// </summary>
    public class AutomaticPlayer : IPlayer
    {
        internal SerializableMessage prevRcvCmd;
        public AutomaticPlayer()
        {
            prevRcvCmd = new ExitingRequestMsg();
        }

        public override List<Point> DeployShips()
        {
            BattleArea ba = new BattleArea(Game.width, Game.height);
            Random rand = new Random();
            foreach (var item in Game.ShipsToDeploy)
            {
                while (!ba.SetShipToPointWhenNoOverlap(item, rand.Next(Game.width), rand.Next(Game.height)))
                    ;
            }
            return ba.map;
        }

        public override bool DoTurn()
        {
           if(Game.battleArea.map.All((p)=>p.ship == Game.ShipType.Single(s => s.Type == Game.Null)))
            {
                //敗戦処理
                Logger.WriteLine("敗北しました。");
                Messenger.Send(new ExitingRequestMsg().ToString());
                var manufactedMsg = MessageFactory.Manufact(Messenger.Recieve());
                Debug.Assert(manufactedMsg.MsgId == MessageId.ExitingResponse);
                return true;
            }

           //コマンド選択率設定
            Dictionary<MessageId, int> electionProb = new Dictionary<MessageId, int> {
               { MessageId.FiringRequest, 1 },
               { MessageId.MovingRequest, 0 },
           };

            MessageId selected = MessagePercentageChoice(electionProb);
            switch (selected)
            {
                case MessageId.FiringRequest:
                    FiringRequest();
                    break;
                case MessageId.MovingRequest:
                    MovingRequest();
                    break;
            }

            return false;
        }

        private void MovingRequest()
        {
            throw new NotImplementedException();
        }

        private void FiringRequest()
        {
            List<Point> lp = new List<Point>();
            foreach (var point in Game.battleArea.map.Where(p=>p.ship != Game.ShipType.Single(s=>s.Type==Game.Null)))
            {
                lp.AddRange(Game.GetPointsShipInPointCanShoot(point));
            }
            Random rand = new Random();
            Point target =  prevRcvCmd.MsgId == MessageId.FiringRequest ? Game.GetPoint( ((FiringRequestMsg)prevRcvCmd).x, ((FiringRequestMsg)prevRcvCmd).y)  :  lp[rand.Next(lp.Count)];
            Debug.Assert(Game.IsInRange(target.x, target.y));
            var req = new FiringRequestMsg(target.x, target.y);
            Messenger.Send(req.ToString());
            Logger.WriteAndDisplay($"地点({target.x}, {target.y})を砲撃しました。");

            var msg = MessageFactory.Manufact(Messenger.Recieve());
            Debug.Assert(msg.MsgId == MessageId.FiringResponse);
            FiringResponseMsg res = (FiringResponseMsg)msg;
            switch (res.summary)
            {
                case FiringResponseSummary.Hit:
                    if (res.destroyedName != Game.Null)
                    {
                        Logger.WriteLine($"{res.destroyedName}を撃沈しました！");
                    }
                    else
                    {
                        Logger.WriteLine("敵艦船に直撃しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    Logger.WriteLine("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    Logger.WriteLine("海に落ちました。");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 割合に応じて要素をランダムに選択します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <see cref="http://qiita.com/haagiii/items/30c917746bc2983be511"/>
        internal MessageId MessagePercentageChoice(Dictionary<MessageId, int> target)
        {
            // まず、全体の合計を求める
            int total = 0;
            foreach (var i in target) total += i.Value;

            // [0, total)の整数をランダムに取得
            var _rand = (new Random()).Next(0, total);

            int temp = 0;
            var res = MessageId.None;
            foreach (var i in target)
            {
                temp += i.Value;
                if (_rand < temp) // あまり直感的では無いかもしれないけど、要は、乱数がどの区間に収まっているのかを探しているだけ 
                {
                    res = i.Key;
                    break; // breakとかしないで、return i.Key;でもよい。
                }
            }

            return res;
        }

        public override bool DoResponse()
        {
            string msg = Messenger.Recieve();
            SerializableMessage recieved = MessageFactory.Manufact(msg);
            switch (recieved.MsgId)
            {
                case MessageId.None:
                    break;
                case MessageId.FiringRequest:
                    FiringResponse(recieved as FiringRequestMsg);
                    break;

                case MessageId.MovingRequest:
                    MovingResponse(recieved as MovingRequestMsg);
                    break;

                case MessageId.ExitingRequest:
                    ExitingResponse(recieved as ExitingRequestMsg);
                    break;

                case MessageId.FiringResponse:
                case MessageId.MovingResponse:
                case MessageId.ExitingResponse:
                default:
                    break;
            }

            return recieved.MsgId == MessageId.ExitingRequest;
        }

        private void MovingResponse(MovingRequestMsg msg)
        {
            Logger.WriteLine($"{msg.mover}が{msg.direction}方向に{msg.distance}移動しました。");
            Messenger.Send(new MovingResponseMsg().ToString());
            Logger.WriteLine($"移動に対して応答しました。");
        }

        private void FiringResponse(FiringRequestMsg msg)
        {
            Logger.WriteLine($"地点({msg.x}, {msg.y})が砲撃されました。");
            Debug.Assert(Game.ValidateX(msg.x) && Game.ValidateY(msg.y));
            var send = Game.ShootFromOther(msg.x, msg.y, out Ship hit);
            Messenger.Send(send.ToString());
            switch (send.summary)
            {
                case FiringResponseSummary.Hit:
                    if (send.destroyedName != Game.Null)
                    {
                        Logger.WriteLine($"{send.destroyedName}が撃沈されました..");
                    }
                    else
                    {
                        Logger.WriteLine($"秘匿情報：{hit.Type}に命中しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    Logger.WriteLine("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    Logger.WriteLine("海に落ちました。");
                    break;
                default:
                    break;
            }
        }

        private void ExitingResponse(ExitingRequestMsg msg)
        {
            Logger.WriteLine("終了通知を受け取りました。");
            Messenger.Send(new ExitingResponseMsg().ToString());
            Logger.WriteLine("終了応答を送信しました。");
        }
    }
}
