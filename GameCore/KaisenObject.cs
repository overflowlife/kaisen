using System;
using KaisenLib;
using static KaisenLib.AppSet;

namespace GameCore
{
    /// <summary>
    /// 機雷・デコイなど、設置物を表現します。
    /// </summary>
    public class KaisenObject
    {
        internal string Type;
        internal int Durable;

        internal KaisenObject(string type, int durable)
        {
            Type = type;
            Durable = durable;
        }

        internal KaisenObject(KaisenObject source)
        {
            Type = source.Type;
            Durable = source.Durable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">形式：{Type}{delimiter}{Durable}</param>
        internal KaisenObject(string str)
        {
            string[] split = str.Split(delimiter);
            if (!(split.Length == 2 && int.TryParse(split[1], out int durable)))
            {
                throw new ArgumentException("引数チェックで例外です。");
            }
        }

        public override string ToString()
        {
            return $"{Type}{delimiter}{Durable}";
        }

        public override int GetHashCode()
        {
            return Type[0];
        }
    }
}