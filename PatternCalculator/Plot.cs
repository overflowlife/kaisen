using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;

namespace PatternCalculator
{
   public struct Plot
    {
        public int X;
        public int Y;
        public Plot(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// return  the distance between this and another.
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public int Distance(Plot another)
        {
            return Max(Abs(X - another.X), Abs(Y - another.Y));
        }

        /// <summary>
        /// return the distance between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public int Distance(Plot a, Plot b)
        {
           return  Max(Abs(a.X - b.X), Abs(a.Y - b.Y));
        }

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.X == ((Plot)obj).X && this.Y == ((Plot)obj).Y;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return X * 10 + Y;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }
    }
}
