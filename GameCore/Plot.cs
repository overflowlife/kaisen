using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    internal struct Plot
    {
        internal int X;
        internal int Y;
        internal Plot(int x, int y)
        {
            X = x;
            Y = y;
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

                return this.X == ((Plot)obj).X  && this.Y == ((Plot)obj).Y;
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                return X*10 + Y;
            }
        }
}
