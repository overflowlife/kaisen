using System;
using System.Linq;
using System.Collections.Generic;
namespace GameCore
{
    public class Game
    {
        internal BattleArea battleArea;
        /// <summary>
        /// 本作登場の艦船リストです。リストからの検索にはEnumerable.Single(Func<ship, bool> pred)を利用します。
        /// </summary>
        internal List<Ship> ships { get; }

        /// <summary>
        /// 艦船ごとの配置数です。
        /// </summary>
        internal Dictionary<Ship, int> deployShips { get; }
        internal List<IPlayer> players { get; }
        internal string BB = "戦艦";
        internal string DD = "駆逐艦";
        internal string SS = "潜水艦";
        internal string mine = "機雷";
        internal string Null = "Null";


        public Game()
        {
            battleArea = new BattleArea();
            //登場艦船を生成します。
            ships = new List<Ship> { new Ship(BB, 3, 1, 1, int.MaxValue), new Ship(DD, 2, 1, 1, int.MaxValue), new Ship(SS, 1, 1, 1, int.MaxValue), new Ship(Null, 0, 0, 0, 0) };
            //艦船配置数を決定します
            deployShips = new Dictionary<Ship, int>
            {
                {ships.Single( ship => ship.Type == BB), 1 },
                {ships.Single( ship => ship.Type == DD), 1 },
                {ships.Single( ship => ship.Type == SS), 1 },
            };
            //NullなShipObjを配置します
            battleArea.map.ForEach(p => p.ship = ships.Single(s => s.Type == Null));

        }

        public void Start()
        {
        }
    }
}
