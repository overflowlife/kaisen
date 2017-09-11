using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnAction
{
    static public class Command
    {
        static public void Initialize(List<Pattern> _patterns)
        {
            _patterns.Clear();
            for (int i = 0; i < 15625; i++)
            {
                int s = i % 25;
                int d = (i % (25 * 25)) / 25;
                int b = i / (25 * 25);
                if ((s - d) * (d - b) * (b - s) != 0)
                {
                    _patterns.Add(new Pattern(s, d, b));
                }
            }
        }

        static public void Move()
        {
            //Console.WriteLine("移動した艦艇は何ですか。0:潜水艦,1:駆逐艦,2:戦艦");
            int Ship = int.Parse(Console.ReadLine());
            //Console.WriteLine("移動方向はどちらですか。0:X方向,1:Y方向");
            bool DirectionX = Console.ReadLine() == "0" ? true : false;
            //Console.WriteLine("移動量を符号付きで入力してください。");
            int Distance = int.Parse(Console.ReadLine());

            Program.PastCommands_Active.Add(new PastCommand(true, Ship, Distance * (DirectionX ? 1 : 5), false, 0));
            Program.PastCommands_Passive.Add(new PastCommand(false, 0, 0, false, 0));

            List<int> DeadIDs_ThisTurn_Active = new List<int> { };
            List<int> DeadIDs_ThisTurn_Passive = new List<int> { };

            for (int i = 0; i < Program.Patterns_Active.Count(); i++)
            {
                bool BeforeAlive = Program.Patterns_Active[i].IsAlive;
                Program.Patterns_Active[i].MoveMod(Ship, DirectionX, Distance);
                bool AfterAlive = Program.Patterns_Active[i].IsAlive;
                if (BeforeAlive != AfterAlive)
                {
                    DeadIDs_ThisTurn_Active.Add(i);
                }
            }
            Program.DeadIDs_Active.Add(DeadIDs_ThisTurn_Active);
            Program.DeadIDs_Passive.Add(DeadIDs_ThisTurn_Passive);
        }

        static public void Fire()
        {
            int Ship = 0;
            bool Live = false;
            //Console.WriteLine("0～24の整数値で座標を入力してください。");
            int Coordinate = int.Parse(Console.ReadLine());
            //Console.WriteLine("結果を入力してください。0:直撃,1:ニアミス，2:海に落ちた");
            int Result = int.Parse(Console.ReadLine());
            if (Result == 0)
            {
                Console.WriteLine("その艦艇は沈没しましたか。0:沈没した,1:沈没していない");
                Live = Console.ReadLine() == "0" ? false : true;
                if (!Live)
                {
                    Console.WriteLine("沈没した艦艇は何ですか。0:潜水艦,1:駆逐艦,2:戦艦");
                    Ship = int.Parse(Console.ReadLine());
                }
                Program.PastCommands_Active.Add(new PastCommand(false, 0, 0, false, 0));
                Program.PastCommands_Passive.Add(new PastCommand(false, 0, 0, true, Coordinate));
            }
            else
            {
                Program.PastCommands_Active.Add(new PastCommand(false, 0, 0, false, 0));
                Program.PastCommands_Passive.Add(new PastCommand(false, 0, 0, false, 0));
            }

            List<int> DeadIDs_ThisTurn_Active = new List<int> { };
            List<int> DeadIDs_ThisTurn_Passive = new List<int> { };

            for (int i = 0; i < Program.Patterns_Passive.Count(); i++)
            {
                bool BeforeAlive = Program.Patterns_Passive[i].IsAlive;
                switch(Result)
                {
                    case 0:
                        Program.Patterns_Passive[i].HitMod(Coordinate, Live, Ship);
                        break;
                    case 1:
                    case 2:
                        Program.Patterns_Passive[i].MissDropOrMyShotMod(Coordinate, Result);
                        break;
                }
                bool AfterAlive = Program.Patterns_Passive[i].IsAlive;
                if (BeforeAlive != AfterAlive)
                {
                    DeadIDs_ThisTurn_Passive.Add(i);
                }
            }

            for (int i = 0; i < Program.Patterns_Active.Count(); i++)
            {
                bool BeforeAlive = Program.Patterns_Active[i].IsAlive;
                Program.Patterns_Active[i].MissDropOrMyShotMod(Coordinate, 3);
                bool AfterAlive = Program.Patterns_Active[i].IsAlive;
                if (BeforeAlive != AfterAlive)
                {
                    DeadIDs_ThisTurn_Active.Add(i);
                }
            }

            Program.DeadIDs_Active.Add(DeadIDs_ThisTurn_Active);
            Program.DeadIDs_Passive.Add(DeadIDs_ThisTurn_Passive);
        }

        static public void Inverse()
        {
            PastCommand LastCommand_Active = Program.PastCommands_Active[Program.PastCommands_Active.Count()-1];
            PastCommand LastCommand_Passive = Program.PastCommands_Passive[Program.PastCommands_Passive.Count() - 1];

            if(LastCommand_Active.IsMove)
            {
                for (int i = 0; i < Program.Patterns_Active.Count; i++ )
                {
                    Program.Patterns_Active[i].InvMoveMod(LastCommand_Active.Ship_Move, LastCommand_Active.Increase_Move);
                }
            }
            else
            {
                if(LastCommand_Active.IsHit_Fire)
                {
                    for (int i = 0; i < Program.Patterns_Active.Count; i++)
                    {
                        int Target = Program.Patterns_Active[i].Coordinates.IndexOf(LastCommand_Active.Coordinate_Fire);
                        Program.Patterns_Active[i].InvHitMod(Target);
                    }
                }
            }
            if (LastCommand_Passive.IsMove)
            {
                for (int i = 0; i < Program.Patterns_Passive.Count; i++)
                {
                    Program.Patterns_Passive[i].InvMoveMod(LastCommand_Passive.Ship_Move, LastCommand_Passive.Increase_Move);
                }
            }
            else
            {
                if (LastCommand_Passive.IsHit_Fire)
                {
                    for (int i = 0; i < Program.Patterns_Passive.Count; i++)
                    {
                        int Target = Program.Patterns_Passive[i].Coordinates.IndexOf(LastCommand_Passive.Coordinate_Fire);
                        Program.Patterns_Passive[i].InvHitMod(Target);
                    }
                }
            }

            Program.PastCommands_Active.RemoveAt(Program.PastCommands_Active.Count()-1);
            Program.PastCommands_Passive.RemoveAt(Program.PastCommands_Passive.Count() - 1);

            List<int> LastIDs_Active = Program.DeadIDs_Active[Program.DeadIDs_Active.Count() - 1];
            List<int> LastIDs_Passive = Program.DeadIDs_Passive[Program.DeadIDs_Passive.Count() - 1];
            foreach(int ID in LastIDs_Active)
            {
                Program.Patterns_Active[ID].IsAlive = true;
            }
            foreach (int ID in LastIDs_Passive)
            {
                Program.Patterns_Passive[ID].IsAlive = true;
            }
            Program.DeadIDs_Active.RemoveAt(Program.DeadIDs_Active.Count() - 1);
            Program.DeadIDs_Passive.RemoveAt(Program.DeadIDs_Passive.Count() - 1);
        }
    }
}
