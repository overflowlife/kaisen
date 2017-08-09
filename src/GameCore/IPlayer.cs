using System.Collections.Generic;
namespace GameCore
{
    internal interface IPlayer
    {
        string Name { get; set; }
        /// <summary>
        /// 艦船を配置し、配置したあとの座標リストを返却します。
        /// </summary>
        /// <returns></returns>
        List<Point> deployShips();
        /// <summary>
        /// こちら側のコマンドを発信し、相手側の応答を受け取ります。
        /// </summary>
        /// <returns>終了フラグ</returns>
        bool DoTurn();
        /// <summary>
        /// 相手側のコマンドを受け取り、処理し、こちら側の応答を発信します。
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>終了フラグ</returns>
        bool Recieve(string msg);
    }
}