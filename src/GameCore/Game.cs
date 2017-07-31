using System.Linq;
namespace GameCore
{
    public class Game
    {
        private BattleArea battleArea;

        public Game()
        {
            battleArea = new BattleArea();
            System.Console.WriteLine(new FiringMsg(0, 0, battleArea.ships.Single(ship => ship.Type == "戦艦")).ToString());
        }
    }
}
