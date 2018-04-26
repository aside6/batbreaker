using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatBreaker2.Models;
using CommonLibrary;

namespace BatBreaker.Controllers
{
    public class MyPlayerController : Controller
    {
        public ActionResult MyPlayerInfo()
        {
            BatBreakerContext db = new BatBreakerContext();
            int userId = User.plainUserId();
            Player pl = db.Players.Where(p => p.userId.Equals(userId)).FirstOrDefault();

            return View(pl);
        }

        public ActionResult MyPlayerAttributes()
        {
            BatBreakerContext db = new BatBreakerContext();
            int userId = User.plainUserId();
            PlayerAttribute pa = db.PlayerAttributes.Where(p => p.Player.userId.Equals(userId)).FirstOrDefault();
            
            return View(pa);
        }

        public ActionResult EditPlayerAttributes(int id)
        {
           BatBreakerContext db = new BatBreakerContext();
           PlayerAttribute pa = db.PlayerAttributes.Where(p => p.playerId.Equals(id)).FirstOrDefault();
           if (pa.Player.userId == User.plainUserId())
           {
               return View(pa);
           }
           else
           {
               return View();
           }            
           
        }

        [HttpPost]
        public ActionResult EditPlayerAttributes(int id, PlayerAttribute model)
        {
            BatBreakerContext db = new BatBreakerContext();
            PlayerAttribute pa = db.PlayerAttributes.Where(p => p.playerId.Equals(id)).FirstOrDefault();
            pa.Player.Points = Request.Form["txtPoints"].ToInt();
            pa.ArmAccuracy = model.ArmAccuracy;
            pa.ArmPower = model.ArmPower;
            pa.BattingClutch = model.BattingClutch;
            pa.Blocking = model.Blocking;
            pa.BRAbility = model.BRAbility;
            pa.BRAgressiveness = model.BRAgressiveness;
            pa.ContactVsLeft = model.ContactVsLeft;
            pa.ContactVsRight = model.ContactVsRight;
            pa.Durability = model.Durability;
            pa.Fielding = model.Fielding;
            pa.GameCalling = model.GameCalling;
            pa.PlateDiscipline = model.PlateDiscipline;
            pa.PlateVision = model.PlateVision;
            pa.PowerVsLeft = model.PowerVsLeft;
            pa.PowerVsRight = model.PowerVsRight;
            pa.Reaction = model.Reaction;
            pa.Speed = model.Speed;
            pa.Stamina = model.Stamina;

            db.SaveChanges();

            ViewData["Message"] = "Attributes saved";
            return View(pa);
        }

        public ActionResult CreatePlayer()
        {
            BatBreakerContext db = new BatBreakerContext();
            int userId = User.plainUserId();
            Player player = db.Players.Where(p => p.userId.Equals(userId)).FirstOrDefault();

            if (player == null)
            {
                ViewData["Positions"] = db.Positions.Where(p => p.Id > 1).ToList();
                List<string> LR = new List<string>();
                LR.Add("L");
                LR.Add("R");
                ViewData["Arm"] = LR;
                return View(new Player());
            }
            else
            {
                return RedirectToAction("MyPlayerAttributes");
            }
        }

        public ActionResult PlayerPrimary(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            ViewData["Positions"] = db.Positions.Where(p => p.Id > 1).ToList();
            List<string> LR = new List<string>();
            LR.Add("L");
            LR.Add("R");
            ViewData["Arm"] = LR;

            return View(new Player());
        }

        [HttpPost]
        public ActionResult CreatePlayer(Player model)
        {
            BatBreakerContext db = new BatBreakerContext();
            Player p;
            //if (id == 0)
            //{
                Random rnd = new Random();
                List<int> teamIds = db.Teams.Where(t => !t.Players.Any(pl => pl.userId > 0 && pl.positionId.Equals(model.positionId))).Select(t => t.Id).ToList();

                if (teamIds.Count == 0)
                {
                    ViewData["message"] = "<div style=\"font-weight: bold; color: red\">There are no teams available with that position..  Try another position</div>";
                    return View(new Player());
                }

            p = db.Players.Create();
                p.CreateDate = DateTime.Now;
                p.userId = User.plainUserId();
                int teamId = teamIds[rnd.Next(teamIds.Count)];
                Player bp = db.Players.Where(pl => pl.positionId.Equals(model.positionId) && pl.teamId.Equals(teamId) && pl.userId == 0).FirstOrDefault();
                bp.teamId = 0;
                p.teamId = teamId;
                p.Points = 5000;
                db.Players.Add(p);

                PlayerAttribute pa = new PlayerAttribute();
                pa.ArmAccuracy = 35;
                pa.ArmPower = 35;
                pa.BattingClutch = 35;
                pa.Blocking = 35;
                pa.BRAbility = 35;
                pa.BRAgressiveness = 35;
                pa.Confidence = 50;
                pa.ContactVsLeft = 35;
                pa.ContactVsRight = 35;
                pa.Durability = 35;
                pa.Fielding = 35;
                pa.GameCalling = 35;
                pa.PlateDiscipline = 35;
                pa.PlateVision = 35;
                pa.PowerVsLeft = 35;
                pa.PowerVsRight = 35;
                pa.Reaction = 35;
                pa.Speed = 35;
                pa.Stamina = 35;
                p.PlayerAttributes.Add(pa);
            //}
            //else
            //{
            //    p = db.Players.Where(pl => pl.Id.Equals(id)).FirstOrDefault();
            //}

            p.FirstName = model.FirstName;
            p.LastName = model.LastName;
            p.Batting = model.Batting;
            p.Throwing = model.Throwing;
            p.positionId = model.positionId;

            db.SaveChanges();

            return RedirectToAction("MyPlayerAttributes");

        }
    }
}
