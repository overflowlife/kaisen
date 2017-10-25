using System;
using Xunit;
using GameCore;
using System.Collections.Generic;

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
            var calc = new PatternCalculator(true);
            //初期状態のテスト実施済み（13800生成）
            calc.SetActive(true);
            //SetActiveはテスト実施済み
            Plot target = new Plot(2, 2);
            calc.Fire(target, 1, -1);

            calc = null;
        }


    }
}
