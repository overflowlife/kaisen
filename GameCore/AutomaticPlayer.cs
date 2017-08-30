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
        internal MessageId prevRcvCmd;
        public AutomaticPlayer()
        {
            prevRcvCmd = MessageId.None;
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
                Debug.Assert(manufactedMsg.MsgId == MessageId.ExitingResponse, "終了通知に対して異常な応答が返却されました。");
                return true;
            }

            throw new NotImplementedException();

        }

        public override bool DoResponse()
        {
            throw new NotImplementedException();
        }
    }
}
