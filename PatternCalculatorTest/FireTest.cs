using System;
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
        public void Fire22AfterStart()
        {
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
            Assert.All(calc.Friend.Patterns, (p)=>Assert.True(p[0].life > 0));

            calc = null;
        }


    }
}
