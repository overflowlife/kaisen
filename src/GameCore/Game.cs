using System;
using System.Linq;
using System.Collections.Generic;
namespace GameCore
{
    public class Game
    {
        private BattleArea battleArea;
        public List<Ship> ships { get; }//本作登場の艦船リスト
        public Dictionary<Ship, int> deployShips { get; }//艦船ごとの配置数
        public List<IPlayer> players { get; }

        public Game()
        {
            battleArea = new BattleArea();
            ships = new List<Ship> { new Ship("戦艦", 1, 1, int.MaxValue), new Ship("駆逐艦", 1, 1, int.MaxValue), new Ship("潜水艦", 1, 1, int.MaxValue) };
            deployShips = new Dictionary<Ship, int>
            {
                {ships.Single( ship => ship.Type == "戦艦" ), 1 },
                {ships.Single( ship => ship.Type == "駆逐艦"), 1 },
                {ships.Single( ship => ship.Type == "潜水艦"), 1 },
            };
        }

        public void Start()
        {
            
        }
    }
}
