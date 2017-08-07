using System;
using static KaisenLib.AppSet;

namespace GameCore
{
    internal class Ship
    {
        public string Type { get; set; }
        public string Stype { get; set; }
        public int Durable { get; set; }
        public int AttackRange { get; set; }
        public int AttackSpan { get; set; }
        public int MoveSpeed { get; set; }

        internal Ship(string type, int durable, int attackRange, int attackSpan, int moveSpeed)
        {
            Type = type;
            Stype = type.Substring(0, 1); //重複耐性なし
            Durable = durable;
            AttackRange = attackRange;
            AttackSpan = attackSpan;
            MoveSpeed = moveSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">形式：{Type}{delimiter}{Durable}{delimiter}{AttackRange}{delimiter}{AttackSpan}{delimiter}{MoveSpeed}</param>
        internal Ship(string str)
        {
            string[] split = str.Split(delimiter);
            if (split.Length != 5)
                throw new ArgumentException("引数チェックでの例外です。");
            this.Type = split[0];
            this.Stype = this.Type.Substring(0, 1);
            this.Durable = int.Parse(split[1]);
            this.AttackRange = int.Parse(split[2]);
            this.AttackSpan = int.Parse(split[3]);
            this.MoveSpeed = int.Parse(split[4]);
        }

        public override string ToString()
        {
            return $"{Type}{delimiter}{Durable}{delimiter}{AttackRange}{delimiter}{AttackSpan}{delimiter}{MoveSpeed}";
        }

        public override int GetHashCode()
        {
            return Type[0];
        }

    }
}