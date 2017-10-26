using System;
using System.Collections.Generic;
using System.Text;

namespace PatternCalculator
{
    public class Ship
    {
        public Plot plot;
        public int life;
        public int X
        {
            get => plot.X;
        }
        public int Y
        {
            get => plot.Y;
        }
    }
}
