using System;
using System.Collections.Generic;
using System.Text;
using static PatternCalculator.Consts;

namespace PatternCalculator
{
    /// <summary>
    /// 各艦船の配置座標、残耐久値を保持し、戦術マップ1枚分のパターンを表現します。0～2のインデクスにより戦艦、駆逐艦、潜水艦にアクセスできます。
    /// </summary>
    public class Pattern
    {
        private Ship[] Ships { get; set; } = { new Ship(), new Ship(), new Ship() };
        public bool Available { get; set; }

        /// <summary>
        /// 規定の順に位置を与えると、規定の順にHPも割り当てます
        /// </summary>
        /// <param name="bb">戦艦の位置</param>
        /// <param name="dd">駆逐艦の位置</param>
        /// <param name="ss">潜水艦の位置</param>
          public Pattern(Plot bb, Plot dd, Plot ss)
        {
            this[BB].plot = bb;
            this[BB].life = 3;
            this[DD].plot = dd;
            this[DD].life = 2;
            this[SS].plot = ss;
            this[SS].life = 1;
            Available = true;
        }

        /// <summary>
        /// standard indexer to get/set Ship information
        /// </summary>
        /// <param name="target">0..BB, 1..DD, 2..SS</param>
        /// <returns></returns>
        /// 
        public Ship this[int target]
        {
            get => Ships[target];
            set => Ships[target] = value;
        }
    }

}
