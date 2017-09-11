using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnAction
{
    static public class Program
    {

        static public List<Pattern> Patterns_Enemy = new List<Pattern> { };
        static public List<Pattern> Patterns_You = new List<Pattern> { };
        static public List<PastCommand> PastCommands_Enemy = new List<PastCommand> { };
        static public List<PastCommand> PastCommands_You = new List<PastCommand> { };
        static public List<List<int>> DeadIDs_Enemy = new List<List<int>> { };
        static public List<List<int>> DeadIDs_You = new List<List<int>> { };
        static public List<Pattern> Patterns_Active = new List<Pattern> { };
        static public List<Pattern> Patterns_Passive = new List<Pattern> { };
        static public List<PastCommand> PastCommands_Active = new List<PastCommand> { };
        static public List<PastCommand> PastCommands_Passive = new List<PastCommand> { };
        static public List<List<int>> DeadIDs_Active = new List<List<int>> { };
        static public List<List<int>> DeadIDs_Passive = new List<List<int>> { };
        static public List<int> Distribution = new List<int>(25);

        

        static void Main(string[] args)
        {
            Command.Initialize(Patterns_Enemy);
            Command.Initialize(Patterns_You);

            Console.WriteLine("先攻はどちらですか。0:貴方,1:相手");
            bool YourTurn = true;
            switch(int.Parse(Console.ReadLine()))
            {
                case 0:
                    YourTurn = true;
                    break;
                case 1:
                    YourTurn = false;
                    break;
            }
            int i = 0;
            for (int k = 0; k < 25; k++)
            {
                Distribution.Add(0);
            }
            while (true)
            {
                int enempat = Patterns_Enemy.Count((x) => x.IsAlive);
                Console.WriteLine("貴方から見た相手のパターンは{0}通りです。", enempat);
                Console.WriteLine("相手から見た貴方のパターンは{0}通りです。", Patterns_You.Count((x) => x.IsAlive));
                for (int k = 0; k < 25; k++)
                {
                    Distribution[k]=0;
                }
                foreach (var item in Patterns_Enemy.Where(p => p.IsAlive))
                {
                    Distribution[item.Coordinates[0]] += item.LifePoints[0] > 0 ? 1 : 0;
                    Distribution[item.Coordinates[1]] += item.LifePoints[1] > 0 ? 1 : 0;
                    Distribution[item.Coordinates[2]] += item.LifePoints[2] > 0 ? 1 : 0;

                }
                for (int j = 0; j < 25; j++)
                {
                    Distribution[j] = Distribution[j] * 100 / enempat;
                }

                for(int y=0; y<5; y++)
                {
                    for(int x=0; x<5; x++)
                    {
                        string prob = Distribution[x + 5 * y].ToString().PadLeft(3); ;
                        Console.Write($"{prob},");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("{0}のターンです。", YourTurn ? "貴方" : "相手");

                Patterns_Active = YourTurn ? Program.Patterns_You : Program.Patterns_Enemy;
                PastCommands_Active = YourTurn ? Program.PastCommands_You : Program.PastCommands_Enemy;
                DeadIDs_Active = YourTurn ? Program.DeadIDs_You : Program.DeadIDs_Enemy;
                Patterns_Passive = YourTurn ? Program.Patterns_Enemy : Program.Patterns_You;
                PastCommands_Passive = YourTurn ? Program.PastCommands_Enemy : Program.PastCommands_You;
                DeadIDs_Passive = YourTurn ? Program.DeadIDs_Enemy : Program.DeadIDs_You;

                Console.WriteLine("対象者の行動を選んでください。0:移動,1:攻撃,2:一手戻す");
                int Action = int.Parse(Console.ReadLine());
                switch (Action)
                {
                    case 0:
                        Command.Move();
                        break;
                    case 1:
                        Command.Fire();
                        break;
                    case 2:
                        Command.Inverse();
                        break;
                }






                YourTurn = !YourTurn;
            }
        }


    }
}
