using System;
using System.Collections.Generic;
using System.Text;
using PatternCalculator;
using static PatternCalculator.Consts;

namespace PatternCalculatorTest
{
    /// <summary>
    /// it provides methods that necessary for testing by xUnit.NET .
    /// </summary>
    static internal class Static
    {
        /// <summary>
        /// life of the  specified shipshould be at least 1.
        /// </summary>
        static internal bool Alive(Pattern p, int ship)
        {
            return p[ship].life > 0;
        }

        static internal bool AliveAll(Pattern p)
        {
            return Alive(p, BB) && Alive(p, DD) && Alive(p, SS);
        }

        /// <summary>
        /// life of the specified ship should be 0.
        /// </summary>
        static internal bool Dead(Pattern p, int ship)
        {
            return p[ship].life == 0;
        }

        static internal bool DeadAll(Pattern p)
        {
            return Dead(p, BB) && Dead(p, DD) && Dead(p, SS);
        }

        /// <summary>
        /// disrance between center and the specified ship should equal or less than 1.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ship"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        static internal bool Near(Pattern p, int ship, Plot center)
        {
            return p[ship].plot.Distance(center) <= 1;
        }

        /// <summary>
        /// distance between center and the specified ship should be 1.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ship"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        static internal bool Around(Pattern p, int ship, Plot center)
        {
            return p[ship].plot.Distance(center) == 1;
        }

        /// <summary>
        /// plot of the specified ship and center should be equal.
        /// </summary>
        /// <returns></returns>
        static internal bool Equal(Pattern p, int ship, Plot center)
        {
            return p[ship].plot.Distance(center) == 0;
        }

        /// <summary>
        /// distance between center and the specified ship should be bigger than 1.
        /// </summary>
        static internal bool Far(Pattern p, int ship, Plot center)
        {
            return p[ship].plot.Distance(center) > 1;
        }
    }
}
