﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
namespace GameCore
{
    public abstract class IPlayer
    {
        internal ResourceSupplier rs;
        internal Action<string> logging;
        string Name { get; set; }
        /// <summary>
        /// 艦船を配置し、配置したあとの座標リストを返却します。
        /// </summary>
        /// <returns></returns>
        public abstract List<Point> DeployShips();
        /// <summary>
        /// こちら側のコマンドを発信し、相手側の応答を受け取ります。
        /// </summary>
        /// <returns>終了フラグ</returns>
        public abstract bool DoTurn();
        /// <summary>
        /// 相手側のコマンドを受け取り、処理し、こちら側の応答を発信します。
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>終了フラグ</returns>
        public abstract bool DoResponse();
    }
}