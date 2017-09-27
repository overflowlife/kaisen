using System;
using System.Collections.Generic;
using System.Text;
using static GameCore.PatternCalculator.Consts;
using System.Diagnostics;

namespace GameCore
{
    /// <summary>
    /// 敵と味方のパターンリストを保持し、それらを適切に操作します。
    /// </summary>
    /// <remarks></remarks>
    internal class PatternCalculator
    {
        internal PatternSet Friend { get; private set; }
        internal PatternSet Foe{ get; private set; }
        private PatternSet active;
        private PatternSet passive;
        /// <summary>
        /// コンストラクト以外の、各操作にかかった時間を計測するStopwatchです。各操作ごとにリセットします。
        /// </summary>
        internal Stopwatch LastCommand { get; }

        internal PatternCalculator(bool create13800sets)
        {
            LastCommand = new Stopwatch();
            if (create13800sets)
            {
                Friend = new PatternSet(true);
                Foe = new PatternSet(true);
            }
            else
            {
                Friend = null;
                Foe = null;
            }
            active = null;
            passive = null;
        }

        internal PatternCalculator(List<Pattern> friend, List<Pattern> foe) : this(false)
        {
            Friend = new PatternSet(friend);
            Foe = new PatternSet(foe);
        }

        /// <summary>
        /// 敵味方のパターンリストを更新します。
        /// </summary>
        /// <param name="friend"></param>
        /// <param name="foe"></param>
        /// <remarks>こんなのに計測必要ですか？</remarks>
        internal void SetPatterns(List<Pattern> friend, List<Pattern> foe)
        {
            LastCommand.Restart();
            Friend = new PatternSet(friend);
            Foe = new PatternSet(foe);
            LastCommand.Stop();
        }

        /// <summary>
        /// 行動者を教えてください。
        /// </summary>
        /// <param name="isItFriend">行動者は見方ですか？</param>
        internal void SetActive(bool isItFriend)
        {
            LastCommand.Restart();
            if (isItFriend)
            {
                active = Friend;
                passive = Foe;
            }
            LastCommand.Stop();
        }

        /// <summary>
        /// 行動者が砲撃した。
        /// </summary>
        /// <param name="fireTarget"></param>
        /// <param name="summary"></param>
        /// <param name="destroyed"></param>
        internal void Fire(Plot fireTarget, int summary, int destroyed)
        {
            LastCommand.Restart();
            active.Fire(fireTarget);
            passive.Fired(fireTarget, summary, destroyed);
            LastCommand.Stop();
        }

        /// <summary>
        /// 行動者が移動した。
        /// </summary>
        /// <param name="degree"></param>
        /// <param name="dist"></param>
        internal void Move(int degree, int dist)
        {
            LastCommand.Restart();
            active.Move(degree, dist);
            LastCommand.Stop();
        }

        internal void Undo()
        {
            LastCommand.Restart();
            active.Undo();
            passive.Undo();
            LastCommand.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcBase"></param>
        /// <param name="calcTarget"></param>
        /// <returns></returns>
        /// <remarks>これはなに？</remarks>
        static internal bool IsInRange(Plot calcBase, Plot calcTarget)
        {
            return false;
        }

        /// <summary>
        /// パターン計算関係で便利な定数達
        /// </summary>
        static internal class Consts
        {
            internal const int BB = 0;
            internal const int DD = 1;
            internal const int SS = 2;
        }
    }

    /// <summary>
    /// 味方もしくは敵のパターンリスト一式を保持し、それに対する各操作を提供します。[]によりパターンリスト中任意の要素にアクセスできます。
    /// </summary>
    internal class PatternSet
    {
        private List<Pattern> Patterns;
        internal Pattern this[int target]
        {
            get => Patterns[target];
            private set => Patterns[target] = value; //privateでいいかな？
        }
        internal int Count
        {
            get => Patterns.Count;
        }
        /// <summary>
        /// 毎回の操作で取り消されたパターン番号を保持するキュー
        /// </summary>
        private Queue<List<int>> diff;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="create13800set"></param>
        internal PatternSet(bool create13800set)
        {
            diff = new Queue<List<int>>(16);//適当、32は多いかなと
            if (create13800set)
            {
                List<Pattern> initial = new List<Pattern>(13800);//メンバへの直接アクセスは確か比較的に低パフォーマンスなので
                for(int i = 0;i < 25; ++i)
                {
                    for(int j = 0; j < 25; ++j)
                    {
                        for(int k = 0; k < 25; ++k)
                        {
                            if( (i-j) * (j-k) * (k-i) != 0)
                            {
                                initial.Add(new Pattern(new Plot(i % 5, i / 5), new Plot(j % 5, j / 5), new Plot(k % 5, k / 5))); //ここらへんのoperatorは比較的遅いかもしれない
                            }
                        }
                    }
                }
                Patterns = initial;
            } else
            {
                Patterns = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <remarks>この実装本当にあっていますか？コピーしなくていいですか？</remarks>
        internal PatternSet(List<Pattern> list) : this(false)
        {
            Patterns = list;
        }

        #region パターンリストに対する操作
        #region 砲撃された処理一式
        /// <summary>
        /// Fired!
        /// </summary>
        /// <param name="point">impact point</param>
        /// <param name="summary">0..Hit, 1..Nearmiss, 1..Water.</param>
        /// <param name="destroyed">-1..No, 0..BB, 1..DD, 2..SS</param>
        internal void Fired(Plot point, int summary, int destroyed)
        {
            if(summary < 0 || summary > 2)
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
            List<int> dels = new List<int>();//確保量を考えましょう。ターン数とコマンドでおおよそ絞れるかな？
            if(destroyed == -1)
            {// 破壊された艦船がない
                for (int i = 0; i < 13800; ++i)
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
                        { //pointにHP2以上の艦船が配備されていないパターン
                            kill = false;
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
                for (int i = 0; i < 13800; ++i)
                {
                    Pattern target = Patterns[i];
                    if (!target.Available)
                    {
                        continue;
                    }
                    bool kill = true;

                    for(int j = 0; j < 3; ++j)
                    {
                        if ( target[j].plot.Equals(point) && j == destroyed  && --target[j].life == 0  )
                        {//pointにHP1の特定の艦船が配備されていないパターン
                            kill = false;
                        }
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
        internal void  Fire(Plot point)
        {
            List<int> dels = null;

            Debug.Assert(dels != null);
            diff.Enqueue(dels);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degree">方向/パラメータ案内をFired()に倣って書く</param>
        /// <param name="dist">距離</param>
        internal void Move(int degree, int dist)
        {
            List<int> dels = null;

            Debug.Assert(dels != null);
            diff.Enqueue(dels);
        }

        /// <summary>
        /// パターンリストへの直前の操作を取り消します。複数段階の取り消しに対応しますが、Redoには対応していません。
        /// </summary>
        /// <returns>取り戻されたパターン数。-1の場合は取り消せる操作が存在しません（注記：必ずしも初期状態であるとは限らない）。</returns>
        internal int Undo()
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
                    Debug.Assert(!Patterns[target[i]].Available, "削除されていないパターンを復活しようとしました。");
                    Patterns[target[i]].Available = true;
                }
                return target.Count;
            }
        }
#endregion

    }

    /// <summary>
    /// 各艦船の配置座標、残耐久値を保持し、戦術マップ1枚分のパターンを表現します。0～2のインデクスにより戦艦、駆逐艦、潜水艦にアクセスできます。
    /// </summary>
    internal class Pattern
    {
        private Ship[] Ships { get; set; } = { default(Ship), default(Ship), default(Ship) };
        internal bool Available { get; set; }

        /// <summary>
        /// 規定の順に位置を与えると、規定の順にHPも割り当てます
        /// </summary>
        /// <param name="bb">戦艦の位置</param>
        /// <param name="dd">駆逐艦の位置</param>
        /// <param name="ss">潜水艦の位置</param>
        internal Pattern(Plot bb, Plot dd, Plot ss)
        {
            this[BB].plot = bb;
            this[BB].life = 3;
            this[DD].plot = dd;
            this[DD].life = 2;
            this[SS].plot = ss;
            this[SS].life = 1;
            Available = true;
        }

        /// <summary>
        /// standard indexer to get/set Ship information
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// 
        internal Ship this[int target]
        {
            get => Ships[target];
            set =>  Ships[target] = value;
        }

        internal class Ship
        {
            internal Plot plot;
            internal int life;
            internal int X
            {
                get => plot.X;
            }
            internal int Y
            {
                get => plot.Y;
            }
        }
    }


}