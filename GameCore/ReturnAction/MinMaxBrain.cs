using System;
using System.Collections.Generic;
using System.Text;

namespace ReturnAction
{
    /// <summary>
    /// Min-Max法に基づく思考を行います。具体的な評価関数は別途注入します。
    /// 
    /// </summary>
    public class MinMaxBrain
    {
        internal int SearchDepth { get; private set; }
        internal Evaluator engine { get; private set; }
        public MinMaxBrain()
        {
            SearchDepth = 3;
        }

        public MinMaxBrain(Evaluator engine) : this()
        {
            
        }



    }
}
