using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    public class KaisenMsg : IKaisenSerializable
    {
        internal string[] internalMessage { get; private set; }

        public void FromString(string str)
        {
            internalMessage = str.Split(',');
        }

        public override string ToString()
        {
            return string.Join(",", internalMessage);
        }
    }
}
