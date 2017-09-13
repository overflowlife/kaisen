using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    /// <summary>
    /// 現在の盤面状態からMin-Max法に基づく思考を行い、最適と考えられる射撃位置を判断します。。
    /// 
    /// </summary>
    public class MinMaxBrain
    {
        internal int MaxSearchDepth { get; set; }
        internal bool IsStartWithMyTurn { get; set; }
        internal BattleArea InitialFriendBattleArea { get; set; }
        internal BattleArea InitialEnemyBattleArea { get; set; }


        public MinMaxBrain(int maxDepth, bool startWithMyTurn)
        {
            MaxSearchDepth = 10;
        }


        internal double Score(BattleArea area)
        {
            int numFriendPattern = 13800;
            int numFeePattern = 13800;
            return numFriendPattern / numFeePattern;
        }
    }
} 
