using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnAction
{
    public class Pattern
    {
        public List<int> Coordinates;
        public List<int> LifePoints;
        public bool IsAlive { get; set; }

        public Pattern(int _S, int _D, int _B)
        {
            int[] coordinates = { _S, _D, _B };
            Coordinates = new List<int>(coordinates);
            int[] lifepoints = { 1, 2, 3 };
            LifePoints = new List<int>(lifepoints);
            IsAlive = true;
        }

        public void MoveMod(int _ship, bool _directionIsX, int _distance)
        {
            if(_directionIsX)
            {
                int x = Coordinates[_ship] % 5 + _distance;
                if( x < 0 || x > 4)
                {
                    IsAlive = false;
                }
                Coordinates[_ship] += _distance;
            }
            else
            {
                int y = Coordinates[_ship] / 5 + _distance;
                if( y < 0 || y > 4)
                {
                    IsAlive = false;
                }
                Coordinates[_ship] += _distance * 5;
            }
        }

        public void InvMoveMod(int _ship, int _increaseOfCoordinate)
        {
            Coordinates[_ship] -= _increaseOfCoordinate;
        }

        public void HitMod(int _coordinate, bool _live, int _ship)
        {
            if (_live)
            {
                int __ship = Coordinates.IndexOf(_coordinate);
                if(__ship == -1)
                {
                    IsAlive = false;
                }
                else
                {
                    LifePoints[__ship]--;
                    if (LifePoints[__ship] == 0)
                    {
                        IsAlive = false;
                    }
                }
            }
            else
            {
                LifePoints[_ship]--;
                if (_coordinate != Coordinates[_ship] || LifePoints[_ship] != 0)
                {
                    IsAlive = false;
                }
            }
        }

        public void InvHitMod(int _ship)
        {
            if (_ship != -1)
            {
                LifePoints[_ship]++;
            }
        }

        public void MissDropOrMyShotMod(int _coordinate, int _result)
        {
            int Range0 = Math.Max(Math.Abs(_coordinate / 5 - Coordinates[0] / 5), Math.Abs(_coordinate % 5 - Coordinates[0] % 5));
            int Range1 = Math.Max(Math.Abs(_coordinate / 5 - Coordinates[1] / 5), Math.Abs(_coordinate % 5 - Coordinates[1] % 5));
            int Range2 = Math.Max(Math.Abs(_coordinate / 5 - Coordinates[2] / 5), Math.Abs(_coordinate % 5 - Coordinates[2] % 5));
            int MinRange = Math.Min( Math.Min( Range0, Range1), Range2);
            switch(_result)
            {
                case 1:
                    if (MinRange != 1)
                    {
                        IsAlive = false;
                    }
                    break;
                case 2:
                    if (MinRange <= 1)
                    {
                        IsAlive = false;
                    }
                    break;
                case 3:
                    if (MinRange >= 2)
                    {
                        IsAlive = false;
                    }
                    break;
            }
        }
    }
}
