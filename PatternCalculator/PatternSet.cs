using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;
using System.Diagnostics;

namespace PatternCalculator
{

    /// <summary>
    /// 味方もしくは敵のパターンリスト一式を保持し、それに対する各操作を提供します。[]によりパターンリスト中任意の要素にアクセスできます。
    /// </summary>
    public class PatternSet
    {
        public List<Pattern> Patterns;
        public Pattern this[int target]
        {
            get => Patterns[target];
            private set => Patterns[target] = value; //privateでいいかな？
        }
        public int Count
        {
            get
            {
                int available = 0;
                for (int i = 0; i < Patterns.Count; ++i)
                    if (Patterns[i].Available)
                        ++available;
                return available;
            }
        }
        /// <summary>
        /// 毎回の操作で取り消されたパターン番号を保持するキュー//キューじゃないが
        /// </summary>
        //private Queue<List<int>> diff;
       internal Stack<List<int>> diff;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="create13800set"></param>
        public PatternSet(bool create13800set)
        {
            diff = new Stack<List<int>>(32);
            if (create13800set)
            {
                List<Pattern> initial = new List<Pattern>(13800);//メンバへの直接アクセスは確か比較的に低パフォーマンスなので
                for (int i = 0; i < 25; ++i)
                {
                    for (int j = 0; j < 25; ++j)
                    {
                        for (int k = 0; k < 25; ++k)
                        {
                            if ((i - j) * (j - k) * (k - i) != 0)
                            {
                                initial.Add(new Pattern(new Plot(i % 5, i / 5), new Plot(j % 5, j / 5), new Plot(k % 5, k / 5))); //ここらへんのoperatorは比較的遅いかもしれない
                            }
                        }
                    }
                }
                Patterns = initial;
            }
            else
            {
                Patterns = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public PatternSet(List<Pattern> list) : this(false)
        {
            Patterns = new List<Pattern>(list);
        }

        #region パターンリストに対する操作
        #region 砲撃された処理一式
        /// <summary>
        /// Fired!
        /// </summary>
        /// <param name="point">impact point</param>
        /// <param name="summary">0..Hit, 1..Nearmiss, 2..Water.</param>
        /// <param name="destroyed">-1..No, 0..BB, 1..DD, 2..SS</param>
        public void Fired(Plot point, int summary, int destroyed)
        {
            if (summary < 0 || summary > 2)
            {
                throw new ArgumentException();
            }

            List<int> dels = null;

            switch (summary)
            {
                case 0:
                    dels = Hit(point, destroyed);
                    break;
                case 1:
                    dels = Nearmiss(point);
                    break;
                case 2:
                    dels = Water(point);
                    break;
                default:
                    return;
            }

            Debug.Assert(dels != null);
            diff.Push(dels);
        }

        private List<int> Water(Plot point)
        {//point周辺9マスに1隻でも配備されているパターンを削除します。
            List<int> dels = new List<int>();
            for (int i = 0; i < Patterns.Count; ++i)
            {
                var target = Patterns[i];
                if (!target.Available)
                {
                    continue;
                }
                var kill = false;
                for (int j = 0; j < 3; ++j)
                {
                    if ((Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1) && target[j].life > 0)
                    {//point周辺9マスに1隻でもHP1以上の艦船が配備されている
                        kill = true;
                        break;
                    }
                }

                if (kill)
                {
                    dels.Add(i);
                    target.Available = false;
                }
            }
            return dels;
        }

        private List<int> Nearmiss(Plot point)
        {//point周辺8マスに1隻も配備されていないパターンを削除します。
            List<int> dels = new List<int>();
            for (int i = 0; i < Patterns.Count; ++i)
            {
                var target = Patterns[i];
                if (!target.Available)
                {
                    continue;
                }
                var kill = true;

                for (int j = 0; j < 3; ++j)
                {
                    if ((Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1 && !target[j].plot.Equals(point)) && target[j].life > 0)
                    {//if(( 9マス範囲内 && 同位置ではない ) && HPが1以上残っている )
                        kill = false;
                    }
                }

                if (kill)
                {
                    dels.Add(i);
                    target.Available = false;
                }
            }

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
            List<int> dels = new List<int>();//確保量を考えましょう。ターン数とコマンドでおおよそ絞れるかな？
            if (destroyed == -1)
            {// 破壊された艦船がない
                for (int i = 0; i < Patterns.Count; ++i)
                {
                    Pattern target = Patterns[i];
                    if (!target.Available)
                    {
                        continue;
                    }
                    bool kill = true;

                    for (int j = 0; j < 3; ++j)
                    {
                        if ((target[j].plot.Equals(point)) && --target[j].life > 0)
                        { //pointにHP2以上の艦船が配備されているパターン
                            kill = false;
                            break;
                        }
                    }

                    if (kill)
                    {
                        dels.Add(i);
                        target.Available = false;
                    }
                }
            }
            else
            {// 破壊された艦船がある
                for (int i = 0; i < Patterns.Count; ++i)
                {
                    Pattern target = Patterns[i];
                    if (!target.Available)
                    {
                        continue;
                    }
                    bool kill = true;

                    if (target[destroyed].plot.Equals(point) && --target[destroyed].life == 0)
                    {//pointにHP1の特定の艦船が配備されているパターン
                        kill = false;
                    }

                    if (kill)
                    {
                        dels.Add(i);
                        target.Available = false;
                    }
                }
            }
            return dels;

        }
        #endregion


        /// <summary>
        /// pointを含む距離1以下の地点に1隻も配備されていないパターンを削除します。
        /// </summary>
        /// <param name="point">target</param>
        public void Fire(Plot point)
        {
            List<int> dels = new List<int>();

            for (int i = 0; i < Patterns.Count; ++i)
            {
                var target = Patterns[i];
                if (!target.Available)
                {
                    continue;
                }
                bool kill = true;

                for (int j = 0; j < 3; ++j)
                {
                    if ((Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1) && target[j].life > 0)
                    {
                        kill = false;
                    }
                }

                if (kill)
                {
                    dels.Add(i);
                    target.Available = false;
                }
            }

            Debug.Assert(dels != null);
            diff.Push(dels);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ship">0..BB, 1..DD, 2..SS</param>
        /// <param name="direction">2, 4, 6, 8</param>
        /// <param name="dist"></param>
        public void Move(int ship, int direction, int dist)
        {
            if (!(direction == 2 || direction == 4 || direction == 6 || direction == 8))
            {
                throw new Exception("方向指定に誤りがあります。");
            }
            List<int> dels = new List<int>();
            for (int i = 0; i < Patterns.Count; ++i)
            {
                var target = Patterns[i];
                if (!target.Available)
                {
                    continue;
                }
                var kill = false;

                if (target[ship].life == 0)
                {
                    kill = true;
                }

                switch (direction)
                {
                    case 4:
                        target[ship].plot = new Plot(target[ship].X - dist, target[ship].Y);
                        break;
                    case 6:
                        target[ship].plot = new Plot(target[ship].X + dist, target[ship].Y);
                        break;
                    case 2:
                        target[ship].plot = new Plot(target[ship].X, target[ship].Y + dist);
                        break;
                    case 8:
                        target[ship].plot = new Plot(target[ship].X, target[ship].Y - dist);
                        break;
                }
                if (target[ship].X < 0 || target[ship].X > 4 || target[ship].Y < 0 || target[ship].Y > 4)
                {
                    kill = true;
                }
                for (int j = 0; j < 3; j++)
                {
                    if (j != ship && target[j].plot.Equals(target[ship].plot))
                    {
                        kill = true;
                        break;
                    }
                }

                if (kill)
                {
                    target.Available = false;
                    dels.Add(i);
                }

            }
            diff.Push(dels);
        }

        /// <summary>
        /// パターンリストへの直前の操作を取り消します。複数段階の取り消しに対応しますが、Redoには対応していません。
        /// </summary>
        /// <returns>取り戻されたパターン数。-1の場合は取り消せる操作が存在しません（注記：必ずしも初期状態であるとは限らない）。</returns>
        public int Undo()
        {
            if (diff.Count == 0)
            {
                return -1;
            }
            else
            {
                List<int> target = diff.Pop();
                for (int i = 0; i < target.Count; ++i)
                {
                    Debug.Assert(!Patterns[target[i]].Available, "削除されていないパターンを復活しようとしました。");
                    Patterns[target[i]].Available = true;
                }
                return target.Count;
            }
        }
        #endregion

    }

}
