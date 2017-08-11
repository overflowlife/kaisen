using KaisenLib;
using static KaisenLib.AppSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    internal class ConsolePlayer : IPlayer
    {
        public string Name { get; set; }
        public Game Game { get; set; }
        internal ConsolePlayer(string name, Game game)
        {
            Name = name;
            Game = game;
        }

        public List<Point> deployShips()
        {
            BattleArea ba = new BattleArea(Game.width, Game.height, Game);
            Logger.WriteAndDisplay("艦船配置オペレーション");
            do
            {
                foreach (var item in Game.ships.Where(ship => ship.Type != Game.Null))
                {
                    for (int i = 0; i < Game.deployShips[item]; ++i)
                    {
                        bool validateInput = false;
                        do
                        {
                            Console.WriteLine($"{item.Type}の配置位置(x, y)を指定してください。");
                            outputArrow("x");
                            string usX = Console.ReadLine();
                            outputArrow("y");
                            string usY = Console.ReadLine();
                            int x, y;

                            bool validateX = int.TryParse(usX, out x) && 0 <= x && x < Game.width;
                            bool validateY = int.TryParse(usY, out y) && 0 <= y && y < Game.height;
                            if(!validateX || !validateY)
                            {
                                Console.WriteLine($"指定位置がマップ境界(0, 0)～（{Game.width}, {Game.height}）を超えています。");
                                continue;
                            }

                            bool validateOverlap = ba.SetShipPointAndSuccess(item, x, y);
                            if (!validateOverlap)
                            {
                                Console.WriteLine($"指定座標にはすでに{ba.GetPoint(x, y).ship.Type}が配置されています。");
                            }

                            validateInput = validateX && validateY && validateOverlap;
                        } while (!validateInput);
                    }
                }
                Console.WriteLine("配置完了しました。最初からやり直しますか？");
                outputArrow("yes: y");
            } while (Console.ReadLine().Trim().ToLower() == "y");

            foreach (var item in ba.map.Where(p => p.ship.Type != Game.Null))
            {
                Logger.WriteAndDisplay($"({item.x}, {item.y})に{item.ship.Type}を配置しました。");
            }

            return ba.map;
        }

        /// <summary>
        /// どうもコマンドキャンセル周りの設計が悪い気がする。cancellationTokenとか使うべき？
        /// </summary>
        /// <returns></returns>
        public bool DoTurn()
        {
            //発行可能なメッセージの定義と、メッセージを発行するメソッドの対応付け
            Dictionary<int, Func<bool>> MsgBinding = new Dictionary<int, Func<bool>>
            {//Dictionary.Keyにenumを使うと遅いらしい
                { (int)KaisenMsgId.FiringRequest, FiringRequest },
                { (int)KaisenMsgId.MovingRequest, MovingRequest },
                { (int)KaisenMsgId.ExitingRequest, ExitingRequest },
            };

            int cmd;
            do
            {
                Func<bool> test;
                bool validateInput;
                do
                {
                    Console.WriteLine("コマンドを選択してください。");
                    foreach (var item in MsgBinding)
                    {
                        Console.WriteLine($"{item.Key}: {(KaisenMsgId)item.Key}");
                    }
                    outputArrow();

                    string input = Console.ReadLine();
                    validateInput = (int.TryParse(input, out cmd) && MsgBinding.TryGetValue(cmd, out test));
                    if (!validateInput)
                    {
                        Console.WriteLine("入力に誤りがあります。");
                    }
                } while (!validateInput);

                
            } while (MsgBinding[cmd].Invoke()); // return if cancelled

            return cmd == (int)KaisenMsgId.ExitingRequest;//微妙
        }

        private bool ExitingRequest()
        {
            Console.WriteLine("ゲームから退出します。よろしいですか？");
            outputArrow("yes: y");
            if(Console.ReadLine().ToLower() == "y")
            {
                Messenger.Send(new ExitingRequestMsg().ToString());
                Logger.WriteAndDisplay("終了通知を発行しました。");
                var manufactedMsg = MsgFactory.Manufact(Messenger.Recieve());
                if (manufactedMsg.msgId == KaisenMsgId.ExitingResponse)
                {
                    Logger.WriteAndDisplay("応答を受け取りました。終了します。");
                    return true;
                }
                else
                    throw new Exception("終了通知に対して異常な応答が返却されました。");
            }
            else
            {
                return false;
            }
            
        }

        private bool CanShoot(int x, int y)
        {
            BattleArea shootingRange = new BattleArea(Game.width, Game.height, Game);

            return true;
        }

        private bool MovingRequest()
        {
            Console.WriteLine(nameof(MovingRequest));
            return false;
        }

        private bool FiringRequest()
        {
            bool cancel = false;

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
                Console.WriteLine("砲撃位置を入力してください。");
                outputArrow("x");
                usX = Console.ReadLine();
                outputArrow("y");
                usY = Console.ReadLine();
                validateX = int.TryParse(usX, out x) && 0 <= x && x <= Game.width;
                validateY = int.TryParse(usY, out y) && 0 <= y && y <= Game.height;
                validateShip = CanShoot(x, y);


                validateInput = validateX && validateY && validateShip;
                if (!validateInput)
                {
                    if (!validateX)
                        Console.WriteLine("xの入力が不正です。");
                    if (!validateY)
                        Console.WriteLine("yの入力が不正です。");
                    if (!validateShip)
                        Console.WriteLine("指定座標は射撃可能範囲外です。");
                }

            } while (!validateInput);

            return cancel;
        }

        public bool Recieve(string msg)
        {
            Console.WriteLine(nameof(Recieve));
            KaisenMsgId cmd = MsgFactory.Manufact(msg).msgId;
            switch (cmd)
            {
                case KaisenMsgId.None:
                    break;
                case KaisenMsgId.FiringRequest:
                    break;

                case KaisenMsgId.MovingRequest:
                    break;

                case KaisenMsgId.ExitingRequest:
                    ExitingResponse(msg);
                    break;

                case KaisenMsgId.FiringResponse:
                case KaisenMsgId.MovingResponse:
                case KaisenMsgId.ExitingResponse:
                default:
                    break;
            }

            return cmd == KaisenMsgId.ExitingRequest;
        }

        private void ExitingResponse(string msg)
        {
            Logger.WriteAndDisplay("終了通知を受け取りました。");
            Messenger.Send(new ExitingResponseMsg().ToString());
        }
    }
}
