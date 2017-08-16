namespace GameCore
{
    /// <summary>
    /// マップ上のある地点のx, y座標、（味方の）存在艦船や設置機雷を保持します。これは機密事項です。
    /// </summary>
    internal class Point
    {
        internal int x;
        internal int y;
        internal Ship ship;
        internal KaisenObject obj;

        internal Point(int x, int y, Ship ship, KaisenObject obj)
        {
            this.x = x;
            this.y = y;
            this.ship = ship;
            this.obj = obj;
        }

        internal Point(Point source)
        {
            this.x = source.x;
            this.y = source.y;
            this.ship = new Ship(source.ship);
            this.obj = new KaisenObject(source.obj);
        }

    }
}