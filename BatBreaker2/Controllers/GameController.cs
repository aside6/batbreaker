using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatBreaker2.Models;
using BatBreaker2.Common;
using CommonLibrary;

namespace BatBreaker.Controllers
{
    public class GameController : Controller
    {
        public JsonResult New()
        {
            return Json(DoNewGame(), JsonRequestBehavior.AllowGet);
        }

        Random rnd = new Random();
        public string DoNewGame(int scheduleId = 0, Random newRnd = null)
        {
            if (newRnd != null)
            {
                rnd = newRnd;
            }
            BatBreakerContext db = new BatBreakerContext();

            //Random rnd = new Random();
            int MyTeamId = 0;

            int userId = User.plainUserId();

            Player MyPlayer = db.Players.Where(p => p.userId.Equals(userId)).FirstOrDefault();
            if (MyPlayer != null)
            {
                MyTeamId = MyPlayer.teamId;
            }

            List<int> teamIds = db.Teams.Select(t => t.Id).ToList();

            Schedule schedule;

            if (scheduleId == 0)
            {
                Game exGame = db.Games.Where(g => (g.awayTeamId == MyTeamId || g.homeTeamId == MyTeamId) && !g.EndDate.HasValue).FirstOrDefault();
                if (exGame != null)
                {
                    return exGame.guId.ToString();
                }
                schedule = db.Schedules.Where(g => (g.awayTeamId == MyTeamId || g.homeTeamId == MyTeamId) && g.Games.FirstOrDefault() == null).FirstOrDefault();
            }
            else
            {
                schedule = db.Schedules.Where(g => g.Id.Equals(scheduleId)).FirstOrDefault();
                if (schedule.Games.FirstOrDefault() != null)
                {
                    return schedule.Games.FirstOrDefault().guId.ToString();
                }
            }

            Game game = db.Games.Create();
            game.guId = Guid.NewGuid();
            game.userId = User.plainUserId();
            game.Outs = 0;
            game.ScoreAway = 0;
            game.ScoreHome = 0;
            game.homeTeamId = schedule.homeTeamId;
            game.awayTeamId = schedule.awayTeamId;
            game.scheduleId = schedule.Id;
            game.StartDate = DateTime.Now;

            db.Games.Add(game);

            Innings inning = db.Innings.Create();
            inning.gameId = game.Id;
            inning.StartDate = DateTime.Now;
            inning.Number = 1;
            inning.TopOfInning = true;

            db.Innings.Add(inning);
            db.SaveChanges();

            IEnumerable<int> numbers = Enumerable.Range(1, 9).OrderBy(r => rnd.Next());

            foreach (int i in numbers)
            {
                GenerateLineup(game.Id, schedule.homeTeamId, i, i, MyPlayer);

                GenerateLineup(game.Id, schedule.awayTeamId, i, i, MyPlayer);
            }

            return game.guId.ToString();
        }

        private void GenerateLineup(int gameId, int teamId, int positionId, int BattingOrder, Player MyPlayer)
        {
            Random rnd = new Random();
            
            BatBreakerContext db = new BatBreakerContext();
            int count = db.Players.Where(p => p.teamId.Equals(teamId) && p.positionId.Equals(positionId)).Count();
            Lineup l = db.Lineups.Create();
            l.gameId = gameId;
            l.teamId = teamId;
            l.positionId = positionId;
            if (MyPlayer != null && teamId == MyPlayer.teamId && positionId == MyPlayer.positionId)
            {
                l.playerId = MyPlayer.Id;
            }
            else
            {
                l.playerId = db.Players.Where(p => p.teamId.Equals(teamId) && p.positionId.Equals(positionId)).ToList()[rnd.Next(0, count)].Id;
            }
            
            l.BattingOrder = BattingOrder;
            db.Lineups.Add(l);
            db.SaveChanges();
        }

        public ActionResult BoxScore(Guid gameId)
        {
            BatBreakerContext db = new BatBreakerContext();
            Game game = db.Games.Where(g => g.guId.Equals(gameId)).FirstOrDefault();

            ViewData["ScoreHome"] = game.ScoreHome;
            ViewData["ScoreAway"] = game.ScoreAway;
            ViewData["HitsHome"] = game.AtBats.Where(ab => !ab.Innings.TopOfInning && ab.resultId > 3).Count();
            ViewData["HitsAway"] = game.AtBats.Where(ab => ab.Innings.TopOfInning && ab.resultId > 3).Count();
            
            return View(game);
        }

        public ActionResult InningScore(IEnumerable<Innings> innings, int Number, bool TopOfInning)
        {
            Innings inning = innings.Where(i => i.Number.Equals(Number) && i.TopOfInning.Equals(TopOfInning)).FirstOrDefault();
            if (inning != null)
            {
                return Content(inning.RunsScoreds.Count.ToString());
            }
            else
            {
                if (innings.First().Game.EndDate.HasValue)
                    return Content("X");
                else
                    return Content("");
            }
        }

        public ActionResult Lineup(Guid id)
        {
            BatBreakerContext db = new BatBreakerContext();
            Game game = db.Games.Where(g => g.guId.Equals(id)).FirstOrDefault();

            GameModels.Lineups lineups = new GameModels.Lineups();

            foreach(var l in game.Lineups.Where(l => l.teamId.Equals(game.homeTeamId)).OrderBy(l => l.BattingOrder)){
                GameModels.TeamLineup tl = new GameModels.TeamLineup();
                tl.BattingOrder = l.BattingOrder;
                tl.playerId = l.playerId;
                tl.PlayerName = l.Player.FirstName + ' ' + l.Player.LastName;
                tl.Position = l.Position.Title;
                lineups.Home.Add(tl);
            }

            foreach(var l in game.Lineups.Where(l => l.teamId.Equals(game.awayTeamId)).OrderBy(l => l.BattingOrder)){
                GameModels.TeamLineup tl = new GameModels.TeamLineup();
                tl.BattingOrder = l.BattingOrder;
                tl.playerId = l.playerId;
                tl.PlayerName = l.Player.FirstName + ' ' + l.Player.LastName;
                tl.Position = l.Position.Title;
                lineups.Away.Add(tl);
            }

            return View(lineups);
        }

        public ActionResult SimulateGame()
        {
            return View();
        }

    }
}
