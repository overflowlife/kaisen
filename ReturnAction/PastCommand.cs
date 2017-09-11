using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnAction
{
    public class PastCommand
    {
        public bool IsMove { get; set; }
        public int Ship_Move { get; set; }
        public int Increase_Move { get; set; }
        public bool IsHit_Fire { get; set; }
        public int Coordinate_Fire { get; set; }

        public PastCommand(bool _isMove, int _ship, int _increase, bool _isHit, int _coordinate_Fire)
        {
            IsMove = _isMove;
            Ship_Move = _ship;
            Increase_Move = _increase;
            IsHit_Fire = _isHit;
            Coordinate_Fire = _coordinate_Fire;
        }
    }
}
