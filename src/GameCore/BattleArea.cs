using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
    internal class BattleArea
    {
        private readonly List<Point> map;
        internal int Width { get; private set; }
        internal int Height { get; private set; }
        internal List<Ship> ships;//本作登場の艦船リスト
        internal Dictionary<Ship, int> deployShips;//艦船ごとの配置数

        internal BattleArea()
        {
            Width = 5;
            Height = 5;
            map = new List<Point>();
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    map[Width * y + x] = new Point(x, y, null, null);
                }
            }
            initShip();
        }

        internal void initShip()
        {
            ships = new List<Ship> { new Ship("戦艦", 1, 1, int.MaxValue), new Ship("駆逐艦", 1, 1, int.MaxValue), new Ship("潜水艦", 1, 1, int.MaxValue) };
            deployShips = new Dictionary<Ship, int>
            {
                {ships.Single( ship => ship.Type == "戦艦" ), 1 },
                {ships.Single( ship => ship.Type == "駆逐艦"), 1 },
                {ships.Single( ship => ship.Type == "潜水艦"), 1 },
            };
        }

        /// <summary>
        /// 指定座標に感染を配置します。
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void SetShipPoint(Ship ship, int x, int y)
        {
            GetPoint(x, y).ship = ship;
        }

        /// <summary>
        /// 指定座標にオブジェクトを設置します。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void SetObjectPoint(KaisenObject obj, int x ,int y)
        {
            GetPoint(x, y).obj = obj;
        }

        /// <summary>
        /// マップ上の特定点を返却します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal Point GetPoint(int x, int y)
        {
            if( 0 <= x && x <= Width && 0<= y && y <= Height)
            {
                return map[x + Width * y];
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}