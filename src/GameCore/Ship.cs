using System;
using static KaisenLib.AppSet;

namespace GameCore
{
     public class Ship
    {
        public string Type { get; set; }
        public string Stype { get; set; }
        public int AttackRange { get; set; }
        public int AttackSpan { get; set; }
        public int MoveSpeed { get; set; }

        public Ship(string type, int attackRange, int attackSpan, int moveSpeed)
        {
            Type = type;
            Stype = type.Substring(0, 1); //重複耐性なし
            AttackRange = attackRange;
            AttackSpan = attackSpan;
            MoveSpeed = moveSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">形式：{Type}{delimiter}{Stype}{delimiter}{AttackRange}{delimiter}{AttackSpan}{delimiter}{MoveSpeed}</param>
        public Ship(string str)
        {
            string[] split = str.Split(delimiter);
            if (split.Length != 5)
                throw new ArgumentException("引数チェックでの例外です。");
            this.Type = split[0];
            this.Stype = split[1];
            this.AttackRange = int.Parse(split[2]);
            this.AttackSpan = int.Parse(split[3]);
            this.MoveSpeed = int.Parse(split[4]);
        }

        public override string ToString()
        {
            return $"{Type}{delimiter}{Stype}{delimiter}{AttackRange}{delimiter}{AttackSpan}{delimiter}{MoveSpeed}";
        }

        public override int GetHashCode()
        {
            return Stype[0];
        }

    }
}