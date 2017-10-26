using System;
using System.Collections.Generic;
using System.Text;
using static PatternCalculator.Consts;
using static System.Math;
using System.Diagnostics;

namespace PatternCalculator
{
    public class Calculator
    {
        public PatternSet Friend { get; private set; } //敵味方それぞれが個別に変更されてしまうのはまずい
        public PatternSet Foe { get; private set; }
        public PatternSet active;
        public PatternSet passive;
        /// <summary>
        /// コンストラクト以外の、各操作にかかった時間を計測するStopwatchです。各操作ごとにリセットします。
        /// </summary>
        public Stopwatch LastCommand { get; }

        public Calculator(bool create13800sets)
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

        public Calculator(List<Pattern> friend, List<Pattern> foe) : this(false)
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
        public void OverwritePatterns(List<Pattern> friend, List<Pattern> foe)
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
        public void SetActive(bool isItFriend)
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
        public void UnSetActive()
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
        public double EstimateFire(Plot point)
        {
            LastCommand.Restart();
            active.Fire(point);
            int activeEstPat = active.Count;
            active.Undo();

            int passivePat = passive.Count;
            int water = 0;
            int nearmiss = 0;
            int hitNoDestroyed = 0;
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

            for (int i = 0; i < 13800; ++i)
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
                if (isNearmiss && !(isHitNo || isHitBb || isHitDd || isHitSs))
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
        /// <param name="summary">0..Hit, 1..Nearmiss, 2..Water.</param>
        /// <param name="destroyed">-1..No, 0..BB, 1..DD, 2..SS</param>
         public   void Fire(Plot fireTarget, int summary, int destroyed)
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
        public void Move(int ship, int direction, int dist)
        {
            LastCommand.Restart();
            active.Move(ship, direction, dist);
            LastCommand.Stop();
        }

        public void Undo()
        {
            LastCommand.Restart();
            active.Undo();
            passive.Undo();
            LastCommand.Stop();
        }
    }

    /// <summary>
    /// パターン計算関係で便利な定数達
    /// </summary>
    static public class Consts
    {
        public const int BB = 0;
        public const int DD = 1;
        public const int SS = 2;
    }
}
