using System;
using Xunit;
using GameCore;

namespace PatternCalculatorTest
{
    public class ConstructorTest
    {
        [Fact]
        public void ConstructCalculatorWithTrueAndAssertGenerate13800Patterns()
        {
            var calc = new PatternCalculator.Calculator(true);
            Assert.True(calc.Friend.Count == 13800);
            Assert.True(calc.Foe.Count == 13800);
            calc = null;
        }

        [Fact]
        public void ConstructCalculatorWithFalseAndAssertNull()
        {
            var calc = new PatternCalculator.Calculator(false);
            Assert.Null(calc.Friend);
            Assert.Null(calc.Foe);
        }
    }
}
