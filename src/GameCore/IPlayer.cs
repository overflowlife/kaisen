namespace GameCore
{
    public interface IPlayer
    {
        void DoTurn();
        void Recieve(string msg);
    }
}