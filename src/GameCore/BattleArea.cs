using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
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
                    map.Add(new Point(x, y, null, null));
                }
            }
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