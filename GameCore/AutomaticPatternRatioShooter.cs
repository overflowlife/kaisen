using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class AutomaticPatternRatioShooter : IPlayer
    {
        private PatternCalculator calculator;

        public AutomaticPatternRatioShooter()
        {
            calculator = new PatternCalculator(true);
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
            /*foreach (var item in ba.map.Where(p => p.ship != null))
            {
                rs.Logger.WriteAndDisplay($"({item.x}, {item.y})に{item.ship.Type}を配置しました。");
            }*/
            return ba.map;
        }

        public override bool DoResponse()
        {
            throw new NotImplementedException();
        }

        public override bool DoTurn()
        {
            System.Threading.Tasks.Task.Delay(500).Wait();//Wait
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
                        rs.Logger.WriteLine($"{res.destroyedName}を撃沈しました！");
                    }
                    else
                    {
                        rs.Logger.WriteLine("敵艦船に直撃しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    rs.Logger.WriteLine("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
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
            foreach (var item in rs.Game.GetPointsWhereCanShoot())
            {
                //各地点評価値の取得
                Plot plot = new Plot(item.x, item.y);
                (int estimatedFriend, int estimatedEnemy) = calculator.EstimateFire(plot);
                estimate += calculator.LastCommand.ElapsedMilliseconds;
                var EvalValue = (double)estimatedFriend / (estimatedFriend + estimatedEnemy);
                EvalVals.Add(plot, EvalValue);
            }
            //vals.Sort();
             

            KeyValuePair<Plot, double> choosen = default(KeyValuePair<Plot, double>);

            foreach (var item in EvalVals)
            {
                if(item.Value > choosen.Value)
                {
                    choosen = item;
                }
            }           
            sw.Stop();
            rs.Logger.WriteAndDisplay($"{sw.ElapsedMilliseconds}msで射撃位置（{choosen.Key.X}, {choosen.Key.Y}）を算出しました。（うち砲撃結果推測時間{estimate}ms）");
            return choosen.Key;
        }
    }
}
