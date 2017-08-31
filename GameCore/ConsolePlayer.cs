using KaisenLib;
using static KaisenLib.AppSet;
using static System.Math;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameCore
{
    /// <summary>
    /// コンソールからの入力によりゲームを進行するプレイヤーです。
    /// </summary>
    public class ConsolePlayer : IPlayer
    {
        internal ResourceSupplier rs;

        public string Name { get; set; }
        public ConsolePlayer(string name, ResourceSupplier rs)
        {
            Name = name;
            this.rs = rs;
        }

        public override List<Point> DeployShips()
        {
            BattleArea ba = new BattleArea(rs.Game.width, rs.Game.height);
            rs.Logger.WriteAndDisplay("艦船配置オペレーション");
            do
            {
                ba = new BattleArea(rs.Game.width, rs.Game.height);
                foreach (var item in rs.Game.ShipsToDeploy)
                {
                    bool validateInput = false;
                    do
                    {
                        Console.WriteLine($"{item.Type}の配置位置(x, y)を指定してください。");
                        OutputArrow("x");
                        string usX = Console.ReadLine();
                        OutputArrow("y");
                        string usY = Console.ReadLine();
                        bool validateX = int.TryParse(usX, out int x) && rs.Game.ValidateX(x) ;
                        bool validateY = int.TryParse(usY, out int y) && rs.Game.ValidateY(y);
                        if(!validateX || !validateY)
                        {
                            Console.WriteLine($"指定位置がマップ境界(0, 0)～（{rs.Game.width}, {rs.Game.height}）を超えています。");
                            continue;
                        }

                        bool validateOverlap = ba.SetShipToPointWhenNoOverlap(item, x, y);
                        if (!validateOverlap)
                        {
                            Console.WriteLine($"指定座標にはすでに{ba.GetPoint(x, y).ship.Type}が配置されています。");
                        }

                        validateInput = validateX && validateY && validateOverlap;
                    } while (!validateInput);
                }
                Console.WriteLine("配置完了しました。最初からやり直しますか？");
                OutputArrow("yes: y");
            } while (Console.ReadLine().Trim().ToLower() == "y");

            foreach (var item in ba.map.Where(p => p.ship != null))
            {
                rs.Logger.WriteAndDisplay($"({item.x}, {item.y})に{item.ship.Type}を配置しました。");
            }

            return ba.map;
        }

        /// <summary>
        /// どうもコマンドキャンセル周りの設計が悪い気がする。
        /// </summary>
        /// <returns></returns>
        public override bool DoTurn()
        {
            //発行可能なメッセージの定義と、メッセージを発行するメソッドの対応付け
            Dictionary<int, Func<bool>> MsgBinding = new Dictionary<int, Func<bool>>
            {//Dictionary.Keyにenumを使うと遅いらしい
                { (int)MessageId.FiringRequest, FiringRequest },
                { (int)MessageId.MovingRequest, MovingRequest },
                { (int)MessageId.ExitingRequest, ExitingRequest },
            };

            int cmdId;
            bool cancel;
            bool exit;
            do
            {
                Func<bool> cmd = () => false;
                bool validateInput;
                DispMap();
                do
                {
                    Console.WriteLine("コマンドを選択してください。");
                    foreach (var item in MsgBinding)
                    {
                        Console.WriteLine($"{item.Key}: {(MessageId)item.Key}");
                    }
                    OutputArrow();

                    string input = Console.ReadLine();
                    validateInput = (int.TryParse(input, out cmdId) && MsgBinding.TryGetValue(cmdId, out cmd));
                    if (!validateInput)
                    {
                        Console.WriteLine("入力に誤りがあります。");
                    }
                } while (!validateInput);
                cancel = cmd.Invoke();// return true if cancelled
                
            } while (cancel);

            exit = (MessageId)cmdId == MessageId.ExitingRequest;
            return exit;
        }

        private bool ExitingRequest()
        {
            Console.WriteLine("ゲームから退出します。よろしいですか？");
            OutputArrow("yes: y");
            if(Console.ReadLine().ToLower() == "y")
            {
                rs.Messenger.Send(new ExitingRequestMsg().ToString());
                rs.Logger.WriteAndDisplay("終了通知を発行しました。");
                var manufactedMsg = MessageFactory.Manufact(rs.Messenger.Recieve());
                Debug.Assert(manufactedMsg.MsgId == MessageId.ExitingResponse, "終了通知に対して異常な応答が返却されました。");
                rs.Logger.WriteAndDisplay("応答を受け取りました。終了します。");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool MovingRequest()
        {
            string usX;
            string usY;
            int x;
            int y;
            bool validateX;
            bool validateY;
            bool validateShip;
            bool validateInput = false;
            do
            {
                Console.WriteLine("移動させる艦船の現在座標を指示して下さい。");
                OutputArrow("x");
                usX = Console.ReadLine();
                OutputArrow("y");
                usY = Console.ReadLine();
                validateX = int.TryParse(usX, out x) && rs.Game.ValidateX(x);
                validateY = int.TryParse(usY, out y) && rs.Game.ValidateX(y);
                validateShip = rs.Game.GetPoint(x, y).ship != null;
                validateInput = validateX && validateY && validateShip;
                if (!validateInput)
                {
                    if (!validateX)
                        Console.WriteLine("xの入力が不正です。");
                    if (!validateY)
                        Console.WriteLine("yの入力が不正です。");
                    if (!validateShip)
                        Console.WriteLine("指定座標に艦船が存在しません。");
                }
            } while (!validateInput);

            string usDir;
            string usDis;
            int dir;
            int dis;
            bool validateDir;
            bool validateDis;
            bool validateOverlap;
            do
            {
                Console.WriteLine("移動方向、移動距離（>0）を指示してください。");
                OutputArrow("Dir:2,4,6or8");
                usDir = Console.ReadLine();
                OutputArrow("Dis");
                usDis = Console.ReadLine();
                validateDir = int.TryParse(usDir, out dir) && dir == 2 || dir == 4 || dir == 6 || dir == 8;
                validateDis = int.TryParse(usDis, out dis) && dis <= rs.Game.GetPoint(x, y).ship.MoveSpeed &&
                    (dir == 4 || dir == 6) ?
                    (0 < dis &&  dis < rs.Game.width ) :
                    (0 < dis &&  dis < rs.Game.height );

                if(dir == 4 || dir == 6)
                {
                    int movedX = dir == 4 ? (x - dis) : (x + dis);
                    validateShip = rs.Game.ValidateX(movedX);
                    validateOverlap = validateShip && rs.Game.GetPoint(movedX, y).ship == null;//ブロックをこれ以上増やさないため

                }
                else
                {
                    int movedY = dir == 2 ? (y + dis) : (y - dis);
                    validateShip = rs.Game.ValidateY(movedY);
                    validateOverlap = validateShip && rs.Game.GetPoint(x, movedY).ship == null;
                }
                validateInput = validateDir && validateDis && validateShip && validateOverlap;

                if (!validateInput)
                {
                    if (!validateDir)
                        Console.WriteLine("入力方向が正しくないです。2, 4, 6, 8のいずれかで指示してください。");
                    if (!validateDis)
                        Console.WriteLine("移動距離が0以下、マップ幅以上、もしくは艦船の移動能力以上です。");
                    if (!validateShip)
                        Console.WriteLine("敵前逃亡は認められていません。");
                    if(validateShip && !validateOverlap)
                        Console.WriteLine("移動先にはすでに艦船が存在します。");
                }
            } while (!validateInput);

            Console.WriteLine("移動をキャンセルしますか？");
            OutputArrow("yes: y");
            if (Console.ReadLine().ToLower() == "y")
            {
                return true;
            }

            Point past = new Point(rs.Game.GetPoint(x, y));
            var moveProcess = rs.Game.MoveShip(past.x, past.y, dir, dis);
            Debug.Assert(moveProcess);
            var send = new MovingRequestMsg(dir, dis, past.ship.Type);
            rs.Messenger.Send(send.ToString());
            rs.Logger.WriteAndDisplay($"{send.mover}を{send.direction}方向に{send.distance}だけ移動しました。");
            var rec =  MessageFactory.Manufact(rs.Messenger.Recieve());
            Debug.Assert(rec.MsgId == MessageId.MovingResponse);
            rs.Logger.WriteAndDisplay("移動に対する応答を受け取りました。");

            return false; 
        }

        private bool FiringRequest()
        {
            string usX;
            string usY;
            int x;
            int y;
            bool validateX;
            bool validateY;
            bool validateRange;
            bool validateInput = false;
            do
            {
                Console.WriteLine("砲撃位置を入力してください。");
                OutputArrow("x");
                usX = Console.ReadLine();
                OutputArrow("y");
                usY = Console.ReadLine();
                validateX = int.TryParse(usX, out x) && rs.Game.ValidateX(x);
                validateY = int.TryParse(usY, out y) && rs.Game.ValidateX(y);
                validateRange = rs.Game.IsInRange(x, y);
                validateInput = validateX && validateY && validateRange;
                if (!validateInput)
                {
                    if (!validateX)
                        Console.WriteLine("xの入力が不正です。");
                    if (!validateY)
                        Console.WriteLine("yの入力が不正です。");
                    if (!validateRange)
                        Console.WriteLine("指定座標は射撃可能範囲外です。");
                }
            } while (!validateInput);

            Console.WriteLine("砲撃をキャンセルしますか？");
            OutputArrow("yes: y");
            if(Console.ReadLine().ToLower() == "y")
            {
                return true;
            }

            var req = new FiringRequestMsg(x, y);
            rs.Messenger.Send(req.ToString());
            rs.Logger.WriteAndDisplay($"地点({x}, {y})を砲撃しました。");
            var msg = MessageFactory.Manufact(rs.Messenger.Recieve());
            Debug.Assert(msg.MsgId == MessageId.FiringResponse, "砲撃通知に対して異常な応答が返却されました。");
            FiringResponseMsg res = (FiringResponseMsg)msg;
            switch (res.summary)
            {
                case FiringResponseSummary.Hit:
                    if(res.destroyedName != ""){
                        rs.Logger.WriteAndDisplay($"{res.destroyedName}を撃沈しました！");
                    }else{
                        rs.Logger.WriteAndDisplay("敵艦船に直撃しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    rs.Logger.WriteAndDisplay("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    rs.Logger.WriteAndDisplay("海に落ちました。");
                    break;
                default:
                    break;
            }


            return false;
        }

        public override bool DoResponse()
        {
            Console.WriteLine("Wait..");
            string msg = rs.Messenger.Recieve();
            SerializableMessage recieved = MessageFactory.Manufact(msg);
            switch (recieved.MsgId)
            {
                case MessageId.None:
                    break;
                case MessageId.FiringRequest:
                    FiringResponse(recieved as FiringRequestMsg );
                    break;

                case MessageId.MovingRequest:
                    MovingResponse(recieved as MovingRequestMsg);
                    break;

                case MessageId.ExitingRequest:
                    ExitingResponse( recieved as ExitingRequestMsg);
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
            rs.Logger.WriteAndDisplay($"{msg.mover}が{msg.direction}方向に{msg.distance}移動しました。");
            rs.Messenger.Send(new MovingResponseMsg().ToString());
            rs.Logger.WriteAndDisplay($"移動に対して応答しました。");
        }

        private void FiringResponse(FiringRequestMsg msg)
        {
            rs.Logger.WriteAndDisplay($"地点({msg.x}, {msg.y})が砲撃されました。");
            Debug.Assert(rs.Game.ValidateX(msg.x) && rs.Game.ValidateY(msg.y));
            var send = rs.Game.ShootFromOther(msg.x, msg.y, out Ship hit);
            rs.Messenger.Send(send.ToString());
            switch (send.summary)
            {
                case FiringResponseSummary.Hit:
                    if(send.destroyedName != "")
                    {
                        rs.Logger.WriteAndDisplay($"{send.destroyedName}が撃沈されました..");
                    }
                    else
                    {
                        rs.Logger.WriteAndDisplay($"秘匿情報：{hit.Type}に命中しました。");
                    }
                    break;
                case FiringResponseSummary.Nearmiss:
                    rs.Logger.WriteAndDisplay("ニアミスでした。");
                    break;
                case FiringResponseSummary.Water:
                    rs.Logger.WriteAndDisplay("海に落ちました。");
                    break;
                default:
                    break;
            }
        }

        private void ExitingResponse(ExitingRequestMsg msg)
        {
            rs.Logger.WriteAndDisplay("終了通知を受け取りました。");
            rs.Messenger.Send(new ExitingResponseMsg().ToString());
            rs.Logger.WriteAndDisplay("終了応答を送信しました。");
        }

        private void DispMap()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"「{Name}」戦術画面");
            sb.AppendLine();
            sb.AppendLine("健在艦船");
            foreach (var item in rs.Game.battleArea.map.Where(p=>p.ship != null))
            {
                sb.AppendLine($"{item.ship.Type}({item.x}, {item.y})：{item.ship.Durable}");
            }
            sb.Append(' ');
            for (int i = 0; i < rs.Game.width; ++i)
            {
               sb.Append(' ' + i.ToString());
            }
            sb.AppendLine();
            for(int y = 0; y < rs.Game.height; ++y)
            {
                sb.Append(y);
                for(int x= 0; x < rs.Game.width; ++x)
                {
                    string type = rs.Game.GetPoint(x, y).ship != null ? rs.Game.GetPoint(x, y).ship.Stype : "　";
                    sb.Append(type);
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            Console.Write(sb.ToString());
        }
    }
}
