namespace GameCore
{
    public interface IKaisenSerializable
    {
        string ToString();
        void FromString(string str);
    }
}