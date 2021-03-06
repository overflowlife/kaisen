﻿namespace GameCore
{
    /// <summary>
    /// マップ上のある地点のx, y座標、（味方の）存在艦船や設置機雷を保持します。これは機密事項です。
    /// </summary>
    public class Point
    {
        public int x { get; internal set; }
        public int y { get; internal set; }
        public Ship ship { get; internal set; }
        public KaisenObject obj { get; internal set; }

        /// <summary>
        /// ordinary construcor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ship"></param>
        /// <param name="obj"></param>
        internal Point(int x, int y, Ship ship, KaisenObject obj)
        {
            this.x = x;
            this.y = y;
            this.ship = ship;
            this.obj = obj;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="source"></param>
        internal Point(Point source)
        {
            x = source.x;
            y = source.y;
            ship = source.ship != null ? new Ship(source.ship) : null;
            obj = source.obj != null ?  new KaisenObject(source.obj) : null;
        }

    }
}