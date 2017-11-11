using System;
using System.Linq;
using Xunit;
using KaisenLib;
using System.Collections.Generic;
using PatternCalculator;
using static PatternCalculator.Consts;
using static PatternCalculatorTest.Static;


namespace PatternCalculatorTest
{
    public class FireTest
    {
        /// <summary>
        /// 開始直後盤面から、友軍が座標(x, y)=(2, 2)に砲撃し、ニアミスを得ます。。
        /// </summary>
        [Fact]
        public void Fire22AndNearmissAfterStart()
        {
            int i = 0;
            /*
            var targets = new List<Plot>();
            for(int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    targets.Add(new Plot(i, j));
                }
            }
            */
            var calc = new Calculator(true);
            //初期状態のテスト実施済み（13800生成）
            calc.SetActive(true);
            //SetActiveはテスト実施済み
            Plot target = new Plot(2, 2);
            calc.Fire(target, 1, -1);
            NearmissTest(i++, calc, target);
            calc = null;
        }

        [Fact]
        public void FireTestNo001()
        {
            int i = 0;
            var calc = new Calculator(true);

            calc.SetActive(false);
            var target = new Plot(2, 2);
            calc.Fire(target, 1, -1);
            NearmissTest(i++, calc, target);
            /*
            calc.SetActive(true);
            target = new Plot(2, 0);
            calc.Fire(target, 0, -1);
            HitTest(i++, calc, target, -1);

            calc.SetActive(false);
            calc.Move(0, 4, 2);
            MoveTest(i++, calc, 0, 4, 2);

            calc.SetActive(true);
            target = new Plot(0, 0);
            calc.Fire(target, 0, -1);
            HitTest(i++, calc, target, -1);

            calc.SetActive(false);
            calc.Move(0, 6, 4);
            MoveTest(i++, calc, target, 0, 6, 4);

            calc.SetActive(true);
            target = new Plot(1, 1);
            calc.Fire(target, 2, -1);
            WaterTest(i++, calc, target);

            calc.SetActive(false);
            target = new Plot(3, 1);
            calc.Fire(target, 2, -1);
            WaterTest(i++, calc, target);
            */

        }


    }
}
