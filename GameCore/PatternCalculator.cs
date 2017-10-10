using System;
using System.Collections.Generic;
using System.Text;
using static GameCore.PatternCalculator.Consts;
using static System.Math;
using System.Diagnostics;

namespace GameCore
{
    /// <summary>
    /// 敵と味方のパターンリストを保持し、それらを適切に操作します。敵味方のパターン登録、行動者設定を行ってください。
    /// </summary>
    /// <remarks></remarks>
    internal class PatternCalculator
    {
        internal PatternSet Friend { get; private set; } //敵味方それぞれが個別に変更されてしまうのはまずい
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
        internal void OverwritePatterns(List<Pattern> friend, List<Pattern> foe)
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
            else
            {
                active = Foe;
                passive = Friend;
            }
            LastCommand.Stop();
        }

        /// <summary>
        /// 行動者を解除します（パターンに対する各操作は無効になります）
        /// </summary>
        /// <remarks>こんなのに計測必要ですか？</remarks>
        internal void UnSetActive()
        {
            LastCommand.Restart();
            active = null;
            passive = null;
            LastCommand.Stop();
        }

        /// <summary>
        /// 行動者が指定位置に砲撃した後のパターン状態を推測し、それに基づいて算出した評価値を返却します。
        /// </summary>
        /// <param name="point"></param>
        /// <returns>（行動者の残パターン数, 待機者の残パターン数）</returns>
        internal double EstimateFire(Plot point)
        {
            LastCommand.Restart();
            active.Fire(point);
            int activeEstPat = active.Count;
            active.Undo();

            int passivePat = passive.Count;
            int water = 0;
            int nearmiss = 0;
            int hitNoDestroyed= 0;
            int hitBbDestroyed = 0;
            int hitDdDestroyed = 0;
            int hitSsDestroyed = 0;
            int totalLife = 0;

            bool isWater = false;
            bool isNearmiss = true;
            bool isHitNo = true;
            bool isHitBb = true;
            bool isHitDd = true;
            bool isHitSs = true;

            for(int i = 0; i < 13800; ++i)
            {
                isWater = true;
                isNearmiss = false;
                isHitNo = false;
                isHitBb = false;
                isHitDd = false;
                isHitSs = false;
                var target = passive[i];
                if (!target.Available)
                {
                    continue;
                }

                for (int j = 0; j < 3; ++j)
                {
                    if ((Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1) && target[j].life > 0)
                    {//point周辺9マスに1隻でもHP1以上の艦船が配備されている
                        isWater = false;
                    }
                    if ((Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1 && !target[j].plot.Equals(point)) && target[j].life > 0)
                    {//if(( 9マス範囲内 && 同位置ではない ) && HPが1以上残っている )
                        isNearmiss = true;
                    }
                    if ((target[j].plot.Equals(point)) && target[j].life > 1)
                    { //pointにHP2以上の艦船が配備されているパターン
                        isHitNo = true;
                    }
                    if (target[j].plot.Equals(point) && target[j].life == 1)
                    {//pointにHP1の特定の艦船が配備されているパターン
                        switch (j)
                        {
                            case 0:
                                isHitBb = true;
                                break;
                            case 1:
                                isHitDd = true;
                                break;
                            case 2:
                                isHitSs = true;
                                break;
                        }
                    }

                }
                byte count = 0;
                if (isWater)
                {
                    ++count;
                    ++water;
                }
                if (isNearmiss && !(isHitNo || isHitBb || isHitDd || isHitSs) )
                {
                    ++count;
                    ++nearmiss;
                }
                if (isHitNo)
                {
                    ++count;
                    ++hitNoDestroyed;
                }
                if (isHitBb)
                {
                    ++count;
                    ++hitBbDestroyed;
                }
                if (isHitDd)
                {
                    ++count;
                    ++hitDdDestroyed;
                }
                if (isHitSs)
                {
                    ++count;
                    ++hitSsDestroyed;
                }
                totalLife += (target[BB].life + target[DD].life + target[SS].life);
                if (isHitNo || isHitBb || isHitDd || isHitSs)
                {
                    --totalLife;
                }
                Debug.Assert(count == 1, "PatternCalculator.EstimateFire()の各条件判断に異常があります");
            }

            double passiveEstPat = ((double)water * water / passivePat)
            + ((double)nearmiss * nearmiss / passivePat)
            + ((double)hitNoDestroyed * hitNoDestroyed / passivePat)
            + ((double)hitBbDestroyed * hitBbDestroyed / passivePat)
            + ((double)hitDdDestroyed * hitDdDestroyed / passivePat)
            + ((double)hitSsDestroyed * hitSsDestroyed / passivePat);
            double passiveEstLife = (double)totalLife / passivePat;

            LastCommand.Stop();
            return (activeEstPat / (activeEstPat + passiveEstPat)) * (1.0D - passiveEstLife / 6.0D);
        }

        /// <summary>
        /// 行動者が砲撃した。
        /// </summary>
        /// <param name="fireTarget"></param>
        /// <param name="summary">0..Hit, 1..Nearmiss, 1..Water.</param>
        /// <param name="destroyed">-1..No, 0..BB, 1..DD, 2..SS</param>
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
        /// <param name="direction"></param>
        /// <param name="dist"></param>
        internal void Move(int ship, int direction, int dist)
        {
            LastCommand.Restart();
            active.Move(ship, direction, dist);
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
            get {
                int available = 0;
                for (int i = 0; i < Patterns.Count; ++i)
                    if (Patterns[i].Available)
                        ++available;
                return available;
            }
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
            diff = new Queue<List<int>>(32);
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
        internal PatternSet(List<Pattern> list) : this(false)
        {
            Patterns = new List<Pattern>(list);
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
            for(int i = 0; i < Patterns.Count; ++i)
            {
                var target = Patterns[i];
                if (!target.Available)
                {
                    continue;
                }
                var kill = true;

                for(int j = 0; j < 3; ++j)
                {
                    if(( Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1 && !target[j].plot.Equals(point) ) && target[j].life > 0)
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
            if(destroyed == -1)
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

                    if ( target[destroyed].plot.Equals(point) && --target[destroyed].life == 0  )
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
        internal void  Fire(Plot point)
        {
            List<int> dels = new List<int>();

            for(int i = 0; i < Patterns.Count; ++i)
            {
                var target = Patterns[i];
                if (!target.Available)
                {
                    continue;
                }
                bool kill = true;

                for(int j = 0; j < 3; ++j)
                {
                    if ( (Max(Abs(target[j].X - point.X), Abs(target[j].Y - point.Y)) <= 1) && target[j].life > 0  )
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
            diff.Enqueue(dels);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ship">0..BB, 1..DD, 2..SS</param>
        /// <param name="direction">2, 4, 6, 8</param>
        /// <param name="dist"></param>
        internal void Move(int ship, int direction, int dist)
        {
            if(!(direction==2||direction==4||direction==6||direction==8))
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
                if( target[ship].X < 0 || target[ship].X > 4 || target[ship].Y < 0 || target[ship].Y > 4 )
                {
                    target.Available = false;
                    dels.Add(i);
                    continue;
                }
                for (int j = 0; j < 3; j++)
                {
                    if(j != ship && target[j].plot.Equals(target[ship]))
                    {
                        target.Available = false;
                        dels.Add(i);
                    }
                }

            }
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
        private Ship[] Ships { get; set; } = { new Ship(), new Ship(), new Ship() };
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
        /// <param name="target">0..BB, 1..DD, 2..SS</param>
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