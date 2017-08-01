using System;
using KaisenLib;

namespace GameCore
{
    internal class Ship
    {
        internal string Type { get; set; }
        internal string Stype { get; set; }
        internal int AttackRange { get; set; }
        internal int AttackSpan { get; set; }
        internal int MoveSpeed { get; set; }

        internal Ship(string type, string sType, int attackRange, int attackSpan, int moveSpeed)
        {
            Type = type;
            Stype = sType;
            AttackRange = attackRange;
            AttackSpan = attackSpan;
            MoveSpeed = moveSpeed;
        }

        /// <summary>
        /// 形式：{Type}{AppSet.delimiter}{Stype}{AppSet.delimiter}{AttackRange}{AppSet.delimiter}{AttackSpan}{AppSet.delimiter}{MoveSpeed}
        /// </summary>
        /// <param name="str"></param>
        internal Ship(string str)
        {
            string[] split = str.Split(AppSet.delimiter);
            if (split.Length != 5)
                throw new ArgumentException();
            this.Type = split[0];
            this.Stype = split[1];
            this.AttackRange = int.Parse(split[2]);
            this.AttackSpan = int.Parse(split[3]);
            this.MoveSpeed = int.Parse(split[4]);
        }

        public override string ToString()
        {
            return $"{Type}{AppSet.delimiter}{Stype}{AppSet.delimiter}{AttackRange}{AppSet.delimiter}{AttackSpan}{AppSet.delimiter}{MoveSpeed}";
        }


    }
}