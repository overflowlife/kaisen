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
            Assert.All(calc.Friend.Patterns.Where((p)=>p.Available), (p)=>Assert.True(AliveAll(p) && (Near(p, BB, target) || Near(p, DD, target) || Near(p, SS, target)), $"{target}�֔��C���܂������A�Y���ʒu���ӂ�1�ǂ����݂��܂���I"));
            Assert.All(calc.Foe.Patterns.Where(p => p.Available), p => Assert.True(AliveAll(p) && (Around(p, BB, target) || Around(p, DD, target) || Around(p, SS, target))  &&!Equal(p, BB, target) && !Equal(p, DD, target) && !Equal(p, SS, target), $"{target}�֖C�����󂯂܂������A�j�A�~�X�����Ɉᔽ���Ă��܂��B�ڍׁF{p}�B"));
            calc = null;
        }
    }
}
