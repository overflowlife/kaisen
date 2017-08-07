namespace GameCore
{
    internal interface IPlayer
    {
        void DoTurn();
        void Recieve(string msg);
    }
}