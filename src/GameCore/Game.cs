﻿using System;
using System.Linq;
using System.Collections.Generic;
using KaisenLib;
using static System.Math;
namespace GameCore
{
    public static class Game
    {
        internal static BattleArea battleArea;
        internal static int height;
        internal static int width;
        /// <summary>
        /// 本作登場の艦船リストです。リストからの検索にはEnumerable.Single(Func<ship, bool> pred)を利用します。
        /// </summary>
        internal static List<Ship> ships { get; }

        /// <summary>
        /// 本作登場の設置物リストです。
        /// </summary>
        internal static List<KaisenObject> objs {get;}
        /// <summary>
        /// 艦船ごとの配置数です。
        /// </summary>
        internal static Dictionary<Ship, int> deployShips { get; }
        /// <summary>
        /// 設置物ごとの配置数です。
        /// </summary>
        internal static Dictionary<KaisenObject, int> deployObjs { get; }
        internal static IPlayer player;
        internal static string BB = "戦艦";
        internal static string DD = "駆逐艦";
        internal static string SS = "潜水艦";
        internal static string mine = "機雷";
        internal static string Null = "Null";//Nullオブジェクトは必要か？

        static Game()
        {
            height = 5;
            width = 5;
            
            //登場艦船を生成します。
            ships = new List<Ship> {
                new Ship(BB, 3, 1, 1, int.MaxValue), //戦艦
                new Ship(DD, 2, 1, 1, int.MaxValue), //駆逐艦
                new Ship(SS, 1, 1, 1, int.MaxValue), //潜水艦
                new Ship(Null, 0, 0, 0, 0)
            };
            //艦船配置数を決定します
            deployShips = new Dictionary<Ship, int>
            {
                {ships.Single( ship => ship.Type == BB), 1 },
                {ships.Single( ship => ship.Type == DD), 1 },
                {ships.Single( ship => ship.Type == SS), 1 },
            };

            //登場設置物を生成します。
            objs = new List<KaisenObject> {
                new KaisenObject(mine, 0),
                new KaisenObject(Null, 0),
            };
            //設置上限数を決定します。
            deployObjs = new Dictionary<KaisenObject, int>
            {
                {objs.Single( obj=> obj.Type==mine), 1 },
            };

            battleArea = new BattleArea(height, width);//登場艦船を生成する前に呼び出してはいけない/悪い設計
        }

        public static void RegisterPlayer(IPlayer player)
        {
            Game.player = player ?? throw new ArgumentNullException();
        }


        public static void DeployShips()
        {
            battleArea.map = player.deployShips(); //プレイヤに艦船を配置させます。
        }

        public static void StartLoop(bool isGuest)
        {
            bool myturn = isMyInitiative(isGuest);
            bool isEnd = false;
            //to fix : infinity loop is dangerous
            while (!isEnd)
            {
                if (myturn)
                {
                    isEnd = player.DoTurn();
                }
                else
                {
                    isEnd = player.Recieve();
                }
                myturn = !myturn;
            }
        }

        /// <summary>
        /// わたしのせんこうですか?
        /// </summary>
        /// <param name="isGuest"></param>
        /// <returns></returns>
        /// <remarks>改善の余地あり</remarks>
        private static bool isMyInitiative(bool isGuest)
        {
            if (isGuest)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool ValidateX(int x)
        {
            return 0 <= x  && x< width;
        }

        internal static bool ValidateY(int y)
        {
            return 0 <= y && y < height;
        }

        internal static Point GetPoint(int x, int y)
        {
            return battleArea.GetPoint(x, y);
        }

        /// <summary>
        /// 指定地点が友軍射程範囲内であるかを判断します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
       internal static bool IsInRange(int x, int y)
        {
            List<Point> lp = new List<Point>();
            //全ての艦船の射程範囲を結合します
            foreach (var p in battleArea.map.Where(p => p.ship.Type != Null))
            {
                var points = GetPointsShipInPointCanShoot(p);
                
                lp.AddRange(points);
            }

            bool canShoot = lp.Any(p => p.x == x && p.y == y);

            return canShoot;
        }

        /// <summary>
        /// 指定地点が友軍射程範囲内であるかを判断します。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static bool IsInRange(Point p)
        {
            return IsInRange(p.x, p.y);
        }

        /// <summary>
        /// target上の艦船が射程範囲に収める地点のリストを返却します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static IEnumerable<Point> GetPointsShipInPointCanShoot(Point target)
        {
            return GetPointsaroundPoint(target, target.ship.AttackRange);
        }

        /// <summary>
        /// target周辺（範囲areaマス）の地点を返却します。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        internal static IEnumerable<Point> GetPointsaroundPoint(Point target, int area)
        {//Euclid距離ではなく、全体の2乗根を外した値で算出しています。
            foreach (var item in battleArea.map)
            {
                var d = Pow(item.x - target.x, 2) + Pow(item.y - target.y, 2);
                if (d < Pow(area+1, 2))
                   yield return item;
            }
        }

        internal static FiringResponseMsg ShootFromOther(int x, int y, out Ship hit)
        {
            FiringResponseSummary summary = FiringResponseSummary.None;
            string destroyed = Null;
            hit = ships.Single(s => s.Type == Null);
            int area = 1;

            if(GetPoint(x, y).ship.Type != Null)
            {//直撃
                summary = FiringResponseSummary.Hit;
                hit = GetPoint(x, y).ship;
                if(--GetPoint(x, y).ship.Durable == 0)
                {//撃沈（耐久値が最初から0以下の場合、それはゾンビ艦船です。撃沈できません。）
                    destroyed = GetPoint(x, y).ship.Type;
                    GetPoint(x, y).ship = ships.Single(s => s.Type == Null);
                }
            } else if(GetPointsaroundPoint(new Point(x, y, null, null), area).Where(p=>!(p.x==x && p.y == y)).Any(p=>p.ship != ships.Single(s=>s.Type==Null)))
            {//ニアミス（砲撃地点周囲から砲撃地点を除外した8マス（射撃範囲1マス時点）中にNullObjectでない艦船を持つ地点が存在す）
                summary = FiringResponseSummary.Nearmiss;
            }
            else
            {//残ケースはポチャン・・・のはず
                summary = FiringResponseSummary.Water;
            }
            return new FiringResponseMsg(summary, destroyed);
        }

        /// <summary>
        /// 地点(x, y)にいる艦船を、dir方向にdis距離だけ移動させます。成功の場合にtrueを返却します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dir"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        internal static bool MoveShip(int x, int y, int dir, int dis)
        {
            Point now = GetPoint(x, y);
            if (now.ship.Type == Null || now.ship.Durable == 0 || now.ship.MoveSpeed < dis)
            {//移動元に艦船が存在しないか、HPが0、もしくは艦船の移動能力を超えた移動量
                return false;
            }

            Point moved;
            if(dir == 4 || dir ==  6)
            {
                int movedX = dir == 4 ? x - dis :x + dis;
                moved = GetPoint(movedX, y);
            }
            else
            {
                int movedY = dir == 2 ? y + dis : y - dis;
                moved = GetPoint(x, movedY);
            }
            if(!ValidateX(moved.x) || !ValidateY(moved.y) || moved.ship.Type != Null)
            {//移動先がマップ範囲を超えている、もしくは移動先にすでに艦船がある
                return false;
            }

            moved.ship = new Ship(now.ship);
            now.ship = ships.Single(s => s.Type == Null);
            return true;
        }
    }
}
