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
            Assert.All(calc.Friend.Patterns.Where((p)=>p.Available), (p)=>Assert.True(AliveAll(p) && (Near(p, BB, target) || Near(p, DD, target) || Near(p, SS, target)), $"{target}へ発砲しましたが、該当位置周辺に1隻も存在しません！"));
            Assert.All(calc.Foe.Patterns.Where(p => p.Available), p => Assert.True(AliveAll(p) && (Around(p, BB, target) || Around(p, DD, target) || Around(p, SS, target))  &&!Equal(p, BB, target) && !Equal(p, DD, target) && !Equal(p, SS, target), $"{target}へ砲撃を受けましたが、ニアミス条件に違反しています。詳細：{p}。"));
            calc = null;
        }
    }
}
