namespace GameCore
{
    internal class Ship
    {
        internal string Type;
        internal string Stype;
        internal int AttackRange;
        internal int AttackSpan;
        internal int MoveSpeed;


        internal Ship(string type, int attackRange, int attackSpan, int moveSpeed)
        {
            Type = type;
            Stype = this.Type.Substring(0, 1); //重複は取り除く必要がある
            AttackRange = attackRange;
            AttackSpan = attackSpan;
            MoveSpeed = moveSpeed;
        }
    }
}