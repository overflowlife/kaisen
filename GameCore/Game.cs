﻿using System;
using System.Linq;
using System.Collections.Generic;
using KaisenLib;
using static System.Math;
namespace GameCore
{
    /// <summary>
    /// ゲーム進行をつかさどります。ToDo:メソッドのありかを再検討しないといけない。　
    /// </summary>
    public class Game
    {
        internal ResourceSupplier rs;
        internal  BattleArea battleArea;
        internal  int height;
        internal  int width;
        /// <summary>
        /// 本作登場の艦種リストです。リストからの検索にはEnumerable.Single(Func<ship, bool> pred)を利用します。
        /// </summary>
        internal  List<Ship> ShipType { get; }

        /// <summary>
        /// 本作登場の設置物リストです。
        /// </summary>
        internal  List<KaisenObject> ObjType {get;}
        /// <summary>
        /// 艦種ごとの配置数です。
        /// </summary>
        internal  Dictionary<Ship, int> NumDeployShips { get; }
        /// <summary>
        /// 艦種リストおよびそれらの配置数から展開した、配置される艦船のリストです。
        /// </summary>
        internal  List<Ship> ShipsToDeploy { get; }
        /// <summary>
        /// 設置物ごとの配置数です。
        /// </summary>
        internal  Dictionary<KaisenObject, int> NumDeployObjs { get; }
        internal  IPlayer Player;
        internal  string BB = "戦艦";
        internal  string DD = "駆逐艦";
        internal  string SS = "潜水艦";
        internal  string mine = "機雷";

        public Game(ResourceSupplier rs)
        {
            this.rs = rs;
            height = 5;
            width = 5;
            
            //登場艦船を生成します。
            ShipType = new List<Ship> {
                new Ship(BB, 3, 1, 1, int.MaxValue), //戦艦
                new Ship(DD, 2, 1, 1, int.MaxValue), //駆逐艦
                new Ship(SS, 1, 1, 1, int.MaxValue), //潜水艦
            };
            //艦船配置数を決定します
            NumDeployShips = new Dictionary<Ship, int>
            {
                {ShipType.Single( ship => ship.Type == BB), 1 },
                {ShipType.Single( ship => ship.Type == DD), 1 },
                {ShipType.Single( ship => ship.Type == SS), 1 },
            };

            ShipsToDeploy = new List<Ship>();
            foreach (var item in ShipType.Where(ship => ship != null))
            {
                for (int i = 0; i < NumDeployShips[item]; ++i)
                {
                    ShipsToDeploy.Add(item);
                }
            }

            //登場設置物を生成します。
            ObjType = new List<KaisenObject> {
                new KaisenObject(mine, 0),
            };
            //設置上限数を決定します。
            NumDeployObjs = new Dictionary<KaisenObject, int>
            {
                {ObjType.Single( obj=> obj.Type==mine), 1 },
            };

            battleArea = new BattleArea(height, width);//登場艦船を生成する前に呼び出してはいけない
        }

        public  void RegisterPlayer(IPlayer player)
        {
            Player = player;
        }

        public  void DeployShips()
        {
            battleArea.map = Player.DeployShips(); //プレイヤに艦船を配置させます。
        }

        public  void StartLoop(bool isGuest)
        {
            bool myturn = IsMyInitiative(isGuest);
            bool isEnd = false;
            //to fix : infinity loop is dangerous
            while (!isEnd)
            {
                if (myturn)
                {
                    isEnd = Player.DoTurn();
                }
                else
                {
                    isEnd = Player.DoResponse();
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
        private static bool IsMyInitiative(bool isGuest)
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

        internal  bool ValidateX(int x)
        {
            return 0 <= x  && x< width;
        }

        internal  bool ValidateY(int y)
        {
            return 0 <= y && y < height;
        }

        internal  Point GetPoint(int x, int y)
        {
            return battleArea.GetPoint(x, y);
        }

        /// <summary>
        /// 指定地点が友軍射程範囲内であるかを判断します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
       internal  bool IsInRange(int x, int y)
        {
            List<Point> lp = new List<Point>();
            //全ての艦船の射程範囲を結合します
            foreach (var p in battleArea.map.Where(p => p.ship != null))
            {
                var points = GetPointsWhereShipOnPointCanShoot(p);
                
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
        internal  bool IsInRange(Point p)
        {
            return IsInRange(p.x, p.y);
        }

        /// <summary>
        /// 射程範囲内の地点リストを返却します。重複する要素がある可能性があります。
        /// </summary>
        /// <returns></returns>
        internal  IEnumerable<Point> GetPointsWhereCanShoot()
        {
            List<Point> lp = new List<Point>();
            foreach (var item in battleArea.map.Where(p=>p.ship != null))
            {
                lp.AddRange(GetPointsWhereShipOnPointCanShoot(item));
            }
            return lp;
        }

        /// <summary>
        /// target上の艦船が射程範囲に収める地点のリストを返却します。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal  IEnumerable<Point> GetPointsWhereShipOnPointCanShoot(Point point)
        {
            return GetPointsaroundPoint(point, point.ship.AttackRange);
        }

        /// <summary>
        /// target周辺（範囲areaマス）の地点を返却します。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        internal  IEnumerable<Point> GetPointsaroundPoint(Point target, int area)
        {
            foreach (var item in battleArea.map)
            {
                if ( Max( Abs(item.x - target.x), Abs(item.y - target.y)) <= area )
                   yield return item;
            }
        }

        internal  FiringResponseMsg ShootFromOther(int x, int y, out Ship hit)
        {
            FiringResponseSummary summary = FiringResponseSummary.None;
            string destroyed = string.Empty;
            hit = null;
            int area = 1;

            if(GetPoint(x, y).ship != null)
            {//直撃
                summary = FiringResponseSummary.Hit;
                hit = GetPoint(x, y).ship;
                if(--GetPoint(x, y).ship.Durable == 0)
                {//撃沈（耐久値が最初から0以下の場合、それはゾンビ艦船です。撃沈できません。）
                    destroyed = GetPoint(x, y).ship.Type;
                    GetPoint(x, y).ship = null;
                }
            } else if(GetPointsaroundPoint(new Point(x, y, null, null), area).Where(p=>!(p.x==x && p.y == y)).Any(p=>p.ship != null))
            {//ニアミス（砲撃地点周囲から砲撃地点を除外した8マス（射撃範囲1マス時点）中にnullでない艦船を持つ地点が存在す）
                summary = FiringResponseSummary.Nearmiss;
            }
            else
            {//残ケースはポチャン・・・のはず
                summary = FiringResponseSummary.Water;
            }
            return new FiringResponseMsg(summary, destroyed, rs);
        }

        /// <summary>
        /// 地点(x, y)にいる艦船を、dir方向にdis距離だけ移動させます。成功の場合にtrueを返却します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dir"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        internal  bool MoveShip(int x, int y, int dir, int dis)
        {
            Point now = GetPoint(x, y);
            if (now.ship== null || now.ship.Durable == 0 || now.ship.MoveSpeed < dis)
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
            if(!ValidateX(moved.x) || !ValidateY(moved.y) || moved.ship != null)
            {//移動先がマップ範囲を超えている、もしくは移動先にすでに艦船がある
                return false;
            }

            moved.ship = new Ship(now.ship);
            now.ship = null;
            return true;
        }
    }
}
