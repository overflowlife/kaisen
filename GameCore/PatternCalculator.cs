using System;
using System.Collections.Generic;
using System.Text;
using static GameCore.PatternCalculator;
using System.Diagnostics;

namespace GameCore
{
    /// <summary>
    /// ハードコーディングくそコード万歳クラス
    /// </summary>
    internal class PatternCalculator
    {

        internal PatternCalculator()
        {
        }

        static internal bool IsInRange()
        {
            return false;
        }
    }

    internal class PatternSet
    {
        List<Pattern> list;
        Queue<List<int>> diff;
        
        internal PatternSet()
        {
            
        }

        //コピー方法の変更必要かも
        internal void SetPattern(List<Pattern> list)
        {
            this.list = list;
        }

        /// <summary>
        /// Fired!
        /// </summary>
        /// <param name="point">impact point</param>
        /// <param name="summary">1..Hit, 2..Nearmiss, 3..Water.</param>
        /// <param name="destroyed">-1..No, 0..BB, 1..DD, 2..SS</param>
        internal void Fired(Plot point, int summary, int destroyed)
        {
            if(summary < 0 || summary > 3)
            {
                return;
            }

            List<int> dels = null;

            switch (summary)
            {
                case 1:
                    dels = Hit(point, destroyed);
                    break;
                case 2:
                    dels = Nearmiss(point);
                    break;
                case 3:
                    dels = Water(point);
                    break;
                default:
                    return;
            }

            Debug.Assert(dels != null);
            diff.Enqueue(dels);
        }

        private List<int> Water(Plot point)
        {
            List<int> dels = new List<int>();
            return dels; 
        }

        private List<int> Nearmiss(Plot point)
        {
            List<int> dels = new List<int>();

            return dels;
        }

        /// <summary>
        /// 破壊された艦船がない場合には、pointにHP2以上の艦船が配備されていないパターンを削除します。
        /// 破壊された艦船がある場合には、pointにHP1の特定の艦船が配備されていないパターンを削除します。
        /// </summary>
        /// <param name="point"></param>
        /// <param name="destroyed"></param>
        /// <returns></returns>
        private List<int> Hit(Plot point, int destroyed)
        {
            List<int> dels = new List<int>();
            for(int i = 0; i < 13800; ++i)
            {
                Pattern target = list[i];
                if (!target.Available)
                {
                    continue;
                }
                if(destroyed == -1) { 
                } else
                {

                }
            }

            return dels;

        }


        /// <summary>
        /// Fire!
        /// </summary>
        /// <param name="point">target</param>
        internal void Fire(Plot point)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degree">方向</param>
        /// <param name="dist">距離</param>
        internal void Move(int degree, int dist)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// パターンリストへの直前の操作を取り消します。
        /// </summary>
        /// <returns>取り戻されたパターン数。-1の場合は取り消せる操作が存在しません。</returns>
        internal int Revert()
        {
            if(diff.Count == 0)
            {
                return -1;
            }
            else
            {
                List<int> target = diff.Dequeue();
                for(int i = 0; i < target.Count; ++i)
                {
                    Debug.Assert(!list[target[i]].Available, "削除されていないパターンを復活しようとしました。");
                    list[target[i]].Available = true;
                }
                return target.Count;
            }
            
        }
    }

    /// <summary>
    /// 各艦船の配置座標、残耐久値を保持し、戦術マップ1枚分のパターンを表現します。
    /// </summary>
    internal class Pattern
    {
        internal Plot[] Plots { get; set; }
        internal int[] Lifes { get; set; }
        internal bool Available { get; set; }


        internal Pattern(Plot[] plots)
        {
            Plots = plots;
            Lifes = new int[] { 1, 2, 3 };
            Available = true;
        }
    }
}