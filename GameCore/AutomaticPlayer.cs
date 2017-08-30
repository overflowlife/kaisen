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

           //コマンド選択率設定
            Dictionary<MessageId, double> electionProb = new Dictionary<MessageId, double> {
               { MessageId.FiringRequest, 0.9 },
               {MessageId.MovingRequest, 0.1 },
           };

            SerializableMessage sendMsg;

            if(prevRcvCmd.MsgId == MessageId.FiringRequest)
            {//直前に射撃通知を受け取っていて、その地点が射程範囲内なら撃ち返す。
                var prevFirReq = (FiringRequestMsg)prevRcvCmd;
                if( Game.IsInRange(prevFirReq.x, prevFirReq.y ) )
                {
                    sendMsg = new FiringRequestMsg(prevFirReq.x, prevFirReq.y);
                }
            }
            else
            {
                MessageId selected;
            }

            throw new NotImplementedException();

        }

        public override bool DoResponse()
        {
            throw new NotImplementedException();
        }
    }
}
