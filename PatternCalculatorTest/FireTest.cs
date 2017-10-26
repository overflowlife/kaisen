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
        /// �J�n����Ֆʂ���A�F�R�����W(x, y)=(2, 2)�ɖC�����A�j�A�~�X�𓾂܂��B�B
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
            //������Ԃ̃e�X�g���{�ς݁i13800�����j
            calc.SetActive(true);
            //SetActive�̓e�X�g���{�ς�
            Plot target = new Plot(2, 2);
            calc.Fire(target, 1, -1);
            Assert.All(calc.Friend.Patterns, (p)=>Assert.True(p[0].life > 0));

            calc = null;
        }


    }
}
