﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaisenLib;
using static KaisenLib.AppSet;
using System.Diagnostics;
using ReturnAction;

namespace GameCore
{
    /// <summary>
    /// ランダム射撃を行う自動応答プレイヤです。
    /// </summary>
    public class AutomaticRandomShootPlayer : IPlayer
    {
        internal SerializableMessage prevRcvCmd;
        public AutomaticRandomShootPlayer(ResourceSupplier rs)
        {
            this.rs = rs;
            prevRcvCmd = new ExitingRequestMsg();
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

            //コマンド選択率設定
            Dictionary<int, int> electionProb = new Dictionary<int, int> {
               { (int)MessageId.FiringRequest, 1 },
               { (int)MessageId.MovingRequest, 0 },
           };

            MessageId selected = (MessageId)MessagePercentageChoice(electionProb);
            switch (selected)
            {
                case MessageId.FiringRequest:
                    FiringRequest();
                    break;
                case MessageId.MovingRequest:
                    MovingRequest();
                    break;
                case (MessageId)(-1):
                    throw new Exception("returned -1 from MessagePercentageChoice.");

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
            foreach (var point in rs.Game.battleArea.map.Where(p => p.ship != null))
            {
                lp.AddRange(rs.Game.GetPointsWhereShipOnPointCanShoot(point));
            }

            Point target = RandomPointWhereIsInRange();

            Debug.Assert(rs.Game.IsInRange(target.x, target.y));
            var req = new FiringRequestMsg(target.x, target.y);
            rs.Messenger.Send(req.ToString());
            rs.Logger.WriteLine($"地点({target.x}, {target.y})を砲撃しました。");

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
        }

        private Point RandomPointWhereIsInRange()
        {
            IEnumerable<Point> lp = rs.Game.GetPointsWhereCanShoot();
            return lp.ElementAt(new Random().Next(lp.Count()));
        }

        /// <summary>
        /// 割合に応じて要素をランダムに選択します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <see cref="http://qiita.com/haagiii/items/30c917746bc2983be511"/>
        internal int MessagePercentageChoice(Dictionary<int, int> target)
        {
            // まず、全体の合計を求める
            int total = 0;
            foreach (var i in target) total += i.Value;

            // [0, total)の整数をランダムに取得
            var _rand = (new Random()).Next(0, total);

            int temp = 0;
            int res = -1;
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
    }
}
