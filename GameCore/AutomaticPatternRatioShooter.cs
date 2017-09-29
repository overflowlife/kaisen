using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var target = bestRatioPoint();

            return false;
        }

        private Plot bestRatioPoint()
        {
            return default(Plot);
        }
    }
}
