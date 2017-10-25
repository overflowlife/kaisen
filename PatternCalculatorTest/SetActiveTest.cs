using System;
using Xunit;
using GameCore;

namespace PatternCalculatorTest
{
    public class SetActiveTest
    {
        [Fact]
        public void SetActiveToFriend()
        {
            var calc = new PatternCalculator(true);
            calc.SetActive(true);
            Assert.True(calc.Friend == calc.active);
            Assert.True(calc.Foe == calc.passive);
            calc = null;
        }

        [Fact]
        public void SetActiveToFoe()
        {
            var calc = new PatternCalculator(true);
            calc.SetActive(false);
            Assert.True(calc.Friend == calc.passive);
            Assert.True(calc.Foe == calc.active);
            calc = null;
        }

        [Fact]
        public void UnsetActive()
        {
            var calc = new PatternCalculator(true);
            calc.UnSetActive();
            Assert.Null(calc.active);
            Assert.Null(calc.passive);
        }
    }
}
