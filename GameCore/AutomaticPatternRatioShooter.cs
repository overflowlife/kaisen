using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PatternCalculator;


namespace GameCore
{
    public class AutomaticPatternRatioShooter : IPlayer
    {
        private Calculator calculator;

        public AutomaticPatternRatioShooter(ResourceSupplier rs)
        {
            this.rs = rs;
            calculator = new Calculator(true);
        }

        public override List<Point> DeployShips()
        {
            BattleArea ba = new BattleArea(rs.Game.width, rs.Game.height);
            Random rand = new Random();
            foreach (var item in rs.Game.ShipsToDeploy)
            {
                while (!ba.SetShipToPointWhenNoOverlap(item, rand.Next(rs.Game.width), rand.Next(rs.Game.height)))
                    ;
            }
            foreach (var item in ba.map.Where(p => p.ship != null))
            {
                rs.Logger.WriteLine($"({item.x}, {item.y})に{item.ship.Type}を配置しました。");
            }
            return ba.map;
        }

        public override bool DoResponse()
        {
            calculator.SetActive(false);
            string msg = rs.Messenger.Recieve();
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

        internal void FiringResponse(FiringRequestMsg msg)
        {
            rs.Logger.WriteLine($"地点({msg.x}, {msg.y})が砲撃されました。");
            Debug.Assert(rs.Game.ValidateX(msg.x) && rs.Game.ValidateY(msg.y));
            var send = rs.Game.ShootFromOther(msg.x, msg.y, out Ship hit);
            rs.Messenger.Send(send.ToString());
            switch (send.summary)
            {
                case FiringResponseSummary.Hit:
                    if (send.destroyedName != string.Empty)
                    {
                        switch (send.destroyedName)
                        {
                            case "戦艦":
                                calculator.Fire(new Plot(msg.x, msg.y), 0, 0);
                                break;
                            case "駆逐艦":
                                calculator.Fire(new Plot(msg.x, msg.y), 0, 1);
                                break;
                            case "潜水艦":
                                calculator.Fire(new Plot(msg.x, msg.y), 0, 2);
                                break;
                            case "Null":
                                calculator.Fire(new Plot(msg.y, msg.y), 0, -1);
                                break;
                            default:
                                throw new Exception("破壊された艦船が異常");
                        }
                        rs.Logger.WriteLine($"{send.destroyedName}が撃沈されました..");
                    }
                    else
                    {
                        rs.Logger.WriteLine($"秘匿情報：{hit.Type}に命中しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    calculator.Fire(new Plot(msg.x, msg.y), 1, -1);
                    rs.Logger.WriteLine("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    calculator.Fire(new Plot(msg.x, msg.y), 2, -1);
                    rs.Logger.WriteLine("海に落ちました。");
                    break;
                default:
                    break;
            }
        }

        internal void MovingResponse(MovingRequestMsg msg)
        {
            rs.Logger.WriteLine($"{msg.mover}が{msg.direction}方向に{msg.distance}移動しました。");
            rs.Messenger.Send(new MovingResponseMsg().ToString());
            int mover;
            if (msg.mover == "戦艦")
            {
                mover = 0;
            }
            else if (msg.mover == "駆逐艦")
            {
                mover = 1;
            }
            else
            {
                mover = 2;
            }
            calculator.Move(mover, msg.direction, msg.distance);
            rs.Logger.WriteLine($"移動に対して応答しました。");
        }

        internal void ExitingResponse(ExitingRequestMsg msg)
        {
            rs.Logger.WriteLine("終了通知を受け取りました。");
            rs.Messenger.Send(new ExitingResponseMsg().ToString());
            rs.Logger.WriteLine("終了応答を送信しました。");
        }

        public override bool DoTurn()
        {
            System.Threading.Tasks.Task.Delay(50).Wait();//Wait
            if (rs.Game.battleArea.map.All((p) => p.ship == null))
            {
                //敗戦処理
                rs.Logger.WriteLine("敗北しました。");
                rs.Messenger.Send(new ExitingRequestMsg().ToString());
                var manufactedMsg = MessageFactory.Manufact(rs.Messenger.Recieve());
                Debug.Assert(manufactedMsg.MsgId == MessageId.ExitingResponse);
                return true;
            }
            calculator.SetActive(true);
            var target = BestRatioPoint();
            Debug.Assert(rs.Game.IsInRange(target.X, target.Y));
            var req = new FiringRequestMsg(target.X, target.Y);
            rs.Messenger.Send(req.ToString());
            rs.Logger.WriteLine($"地点({target.X}, {target.Y})を砲撃しました。");

            var msg = MessageFactory.Manufact(rs.Messenger.Recieve());
            Debug.Assert(msg.MsgId == MessageId.FiringResponse);
            FiringResponseMsg res = (FiringResponseMsg)msg;
            switch (res.summary)
            {
                case FiringResponseSummary.Hit:
                    if (res.destroyedName != string.Empty)
                    {
                        switch (res.destroyedName)
                        {
                            case "戦艦":
                                calculator.Fire(target, 0, 0);
                                break;
                            case "駆逐艦":
                                calculator.Fire(target, 0, 1);
                                break;
                            case "潜水艦":
                                calculator.Fire(target, 0, 2);
                                break;
                            default:
                                throw new Exception("破壊された艦船が異常");
                        }
                        rs.Logger.WriteLine($"{res.destroyedName}を撃沈しました！");
                    }
                    else
                    {
                        rs.Logger.WriteLine("敵艦船に直撃しました。");
                        calculator.Fire(target, 0, -1);
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    calculator.Fire(target, 1, -1);
                    rs.Logger.WriteLine("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    calculator.Fire(target, 2, -1);
                    rs.Logger.WriteLine("海に落ちました。");
                    break;
                default:
                    break;
            }


            return false;
        }

        private Plot BestRatioPoint()
        {
            var sw = Stopwatch.StartNew();
            long estimate = 0L;
            Dictionary<Plot, double> EvalVals = new Dictionary<Plot, double>();
            List<double> vals = new List<double>(rs.Game.GetPointsWhereCanShoot().Count());
            List<Point> used = new List<Point>();
            foreach (var item in rs.Game.GetPointsWhereCanShoot())
            {
                bool skip = false;
                foreach (var item2 in used)
                {
                    if(item.x == item2.x && item.y == item2.y)
                    {
                        skip = true;
                    }
                }
                if (skip)
                {
                    continue;
                }
                else
                {
                    used.Add(item);
                }
                //各地点評価値の取得
                Plot plot = new Plot(item.x, item.y);
                var EvalValue = calculator.EstimateFire(plot);
                estimate += calculator.LastCommand.ElapsedMilliseconds;
                EvalVals.Add(plot, EvalValue);
                vals.Add(EvalValue);
            }
            vals.Sort();
             

            KeyValuePair<Plot, double> choosen = default(KeyValuePair<Plot, double>);

            foreach (var item in EvalVals)
            {
                if(item.Value > choosen.Value)
                {
                    choosen = item;
                }
            }           
            sw.Stop();
            rs.Logger.WriteLine($"{sw.ElapsedMilliseconds}msで射撃位置（{choosen.Key.X}, {choosen.Key.Y}）を算出しました（評価値{choosen.Value}）。（うち砲撃結果推測時間{estimate}ms）");
            return choosen.Key;
        }
    }
}
