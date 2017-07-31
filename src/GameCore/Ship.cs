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

        public override string ToString()
        {
            return $"{Type},{Stype},{AttackRange},{AttackSpan},{MoveSpeed}";
        }

        internal Ship FromString(string str)
        {
            string[] split = str.Split(',');
            if (split.Length != 5)
                return null;
            return new Ship(split[0],split[1], int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4]));
        }
    }
}