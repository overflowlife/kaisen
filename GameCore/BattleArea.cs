using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
    /// <summary>
    /// 幅と高さを持ち、それぞれの座標情報を持った作戦区域を表現します。
    /// </summary>
    internal class BattleArea
    {
        internal List<Point> map;
        internal int Width { get; private set; }
        internal int Height { get; private set; }

        internal BattleArea(int width, int height)
        {
            Width = width;
            Height = height;
            map = new List<Point>(Width * Height);
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    int i = Width * y + x;
                    map.Add(new Point(x, y, Game.ShipType.Single(ship=>ship.Type==Game.Null)  , Game.ObjType.Single(obj=>obj.Type==Game.Null) ));
                }
            }
        }

        /// <summary>
        /// 指定座標に感染を配置します。すでに艦船が配置してある場合には何もせずにfalseを返却します。
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal bool SetShipToPointWhenNoOverlap(Ship ship, int x, int y)
        {
            if(ship == null)
            {
                throw new ArgumentNullException();
            }

            if (GetPoint(x, y).ship != Game.ShipType.Single(s=>s.Type==Game.Null))
                return false; //すでに艦船が存在している座標には配置しない。
            else
            {
                GetPoint(x, y).ship = ship;
                return true;
            }
        }

        /// <summary>
        /// 指定座標に艦船を強制的に配置します。既存の艦船を上書きします。
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void SetShipToPointForcibly(Ship ship, int x, int y)
        {
            if (ship == null)
            {
                throw new ArgumentNullException();
            }

            GetPoint(x, y).ship = ship;
        }

        /// <summary>
        /// 指定座標にオブジェクトを設置します。すでに艦船が配置してある場合には何もせずにfalseを返却します。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal bool SetObjectPointWhenNoOverlap(KaisenObject obj, int x ,int y)
        {
            if(GetPoint(x, y).obj != Game.ObjType.Single(o=>o.Type == Game.Null))
            {
                return false;
            }
            else
            {
                GetPoint(x, y).obj = obj;
                return true;
            }

        }

        /// <summary>
        /// マップ上の特定点を返却します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal Point GetPoint(int x, int y)
        {
            if( 0 <= x && x < Width && 0<= y && y < Height)
            {
                return map[x + Width * y];
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}