using System;
using System.Linq;
using System.Collections.Generic;
using KaisenLib;
namespace GameCore
{
    public class Game
    {
        internal Messenger messenger;
        internal BattleArea battleArea;
        /// <summary>
        /// 本作登場の艦船リストです。リストからの検索にはEnumerable.Single(Func<ship, bool> pred)を利用します。
        /// </summary>
        internal List<Ship> ships { get; }
        /// <summary>
        /// 本作登場の設置物リストです。
        /// </summary>
        internal List<KaisenObject> objs {get;}
        /// <summary>
        /// 艦船ごとの配置数です。
        /// </summary>
        internal Dictionary<Ship, int> deployShips { get; }
        /// <summary>
        /// 設置物ごとの配置数です。
        /// </summary>
        internal Dictionary<KaisenObject, int> deployObjs { get; }
        internal IPlayer me;
        internal IPlayer him;
        internal string BB = "戦艦";
        internal string DD = "駆逐艦";
        internal string SS = "潜水艦";
        internal string mine = "機雷";
        internal string Null = "Null";


        public Game(Messenger messenger)
        {
            this.messenger = messenger;
            battleArea = new BattleArea(5, 5);
            //登場艦船を生成します。
            ships = new List<Ship> {
                new Ship(BB, 3, 1, 1, int.MaxValue), //戦艦
                new Ship(DD, 2, 1, 1, int.MaxValue), //駆逐艦
                new Ship(SS, 1, 1, 1, int.MaxValue), //潜水艦
                new Ship(Null, 0, 0, 0, 0)
            };
            //艦船配置数を決定します
            deployShips = new Dictionary<Ship, int>
            {
                {ships.Single( ship => ship.Type == BB), 1 },
                {ships.Single( ship => ship.Type == DD), 1 },
                {ships.Single( ship => ship.Type == SS), 1 },
            };
            //NullなShipObjを配置します
            battleArea.map.ForEach(p => p.ship = ships.Single(s => s.Type == Null));

            //登場設置物を生成します。
            objs = new List<KaisenObject> {
                new KaisenObject(mine, -1), new KaisenObject(Null, 0)
            };
            //設置上限数を決定します。
            deployObjs = new Dictionary<KaisenObject, int>
            {
                {objs.Single( obj=> obj.Type==mine), 1 },
            };
            //NullなKaisenObjを配置します。
            battleArea.map.ForEach(p => p.obj = objs.Single(o => o.Type == Null));
        }

        public void Start(bool isGuest)
        {
            bool myturn = isMyInitiative(isGuest);
            //to fix : infinity loop is dangerous
            while (true)
            {
                if (myturn)
                {
                    me.DoTurn();
                }else
                {

                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isGuest"></param>
        /// <returns></returns>
        /// <remarks>改善の余地あり</remarks>
        private bool isMyInitiative(bool isGuest)
        {
            if (isGuest)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
