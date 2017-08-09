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
                        bool validateInput;
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
                            bool validatePoint = ba.SetShipPointAndSuccess(item, x, y);
                            validateInput = validateX && validateY && validatePoint;
                            if (!validateInput)
                            {
                                if (!validateX)
                                    Console.WriteLine("X座標に誤りがあります。");
                                if (!validateY)
                                    Console.WriteLine("Y座標に誤りがあります。");
                                if (!validatePoint)
                                    Console.WriteLine("指定座標にすでに艦船が配置されています。");
                            }
                        } while (!validateInput);
                    }
                }
                Console.WriteLine("配置を最初からやり直しますか？");
                outputArrow("yes: y");
            } while (Console.ReadLine().Trim().ToLower() == "y");

            foreach (var item in ba.map.Where(p => p.ship.Type != Game.Null))
            {
                Logger.WriteAndDisplay($"({item.x}, {item.y})に{item.ship.Type}を配置しました。");
            }

            return ba.map;
        }

        public bool DoTurn()
        {
            //発行可能なメッセージの定義と、メッセージを発行するメソッドの対応付け
            Dictionary<int, Func<bool>> MsgBinding = new Dictionary<int, Func<bool>>
            {//Dictionary.Keyにenumを使うと遅いらしい
                { (int)KaisenMsgId.FiringRequest, FiringRequest },
                { (int)KaisenMsgId.MovingRequest, MovingRequest },
                { (int)KaisenMsgId.ExitingRequest, ExitingRequest },
            };

            int usCmd;
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
                    validateInput = (int.TryParse(input, out usCmd) && MsgBinding.TryGetValue(usCmd, out test));
                    if (!validateInput)
                    {
                        Console.WriteLine("入力に誤りがあります。");
                    }
                } while (!validateInput);

                
            } while (MsgBinding[usCmd].Invoke()); // return if cancelled

            return usCmd == (int)KaisenMsgId.ExitingRequest;//微妙
        }

        private bool ExitingRequest()
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
                    if(!validateX)
                        Console.WriteLine("xの入力が不正です。");
                    if(!validateY)
                        Console.WriteLine("yの入力が不正です。");
                    if(!validateShip)
                        Console.WriteLine("指定座標は射撃可能範囲外です。");
                }
                
            } while (!validateInput);
            
            return cancel;
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
            Console.WriteLine(nameof(FiringRequest));
            return false;
        }

        public bool Recieve(string msg)
        {
            Console.WriteLine(nameof(Recieve));
            return false;
        }
    }
}
