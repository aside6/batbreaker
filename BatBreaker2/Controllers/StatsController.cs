using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatBreaker2.Models;
using CommonLibrary;
using BatBreaker2.Common;

namespace BatBreaker.Controllers
{
    public class StatsController : Controller
    {
        public ActionResult Standings()
        {
            BatBreakerContext db = new BatBreakerContext();
            List<StatModels.Standings> Standings = new List<StatModels.Standings>();
            int userId = User.plainUserId();
            Player p = db.Players.Where(pl => pl.userId.Equals(userId)).FirstOrDefault();

            int i = 1;

            foreach(Division div in p.Team.League.Divisions.OrderBy(d => d.Title))
			{
                foreach (Team team in div.Teams)
                {
                    StatModels.Standings divStandings = new StatModels.Standings();
                    divStandings.Division = i;
                    divStandings.Wins = team.HomeGames.Where(h => h.EndDate.HasValue && h.ScoreHome > h.ScoreAway).Count() + team.AwayGames.Where(h => h.EndDate.HasValue && h.ScoreAway > h.ScoreHome).Count();
                    divStandings.Losses = team.HomeGames.Where(h => h.EndDate.HasValue && h.ScoreHome < h.ScoreAway).Count() + team.AwayGames.Where(h => h.EndDate.HasValue && h.ScoreAway < h.ScoreHome).Count();
                    divStandings.team = team;
                    if (divStandings.Wins > 0)
                    {
                        divStandings.WinPct = divStandings.Wins.ToDecimal() / (divStandings.Wins + divStandings.Losses).ToDecimal();
                    }
                    else
                    {
                        divStandings.WinPct = 0M;
                    }

                    Standings.Add(divStandings);
                }
                i++;
               
			}

            return View(Standings);
        }

        public ActionResult GetAllPlayers()
        {
            BatBreakerContext db = new BatBreakerContext();
            List<Player> players = db.Players.ToList();
            

            return View(players);
        }

        public ActionResult ScheduleResults(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            List<Schedule> schedule = db.Schedules.Where(s => s.awayTeamId.Equals(id) || s.homeTeamId.Equals(id)).ToList();
            ViewData["teamId"] = id;

            return View(schedule);
        }

        public ActionResult Team(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            Team team = db.Teams.Where(t => t.Id.Equals(id)).FirstOrDefault();

            return View(team);
        }

        public ActionResult TeamBattingStats(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            var players = db.Players.Where(p => p.teamId.Equals(id)).ToList();

            List<StatModels.BatterStats> stats = new List<StatModels.BatterStats>();
            foreach (Player p in players)
            {
                stats.Add(GetBatterStats(p));
            }

            return View("AllBattingStats", stats.OrderByDescending(s => s.Avg).ToList());
        }

        public ActionResult AllBattingStats()
        {
            BatBreakerContext db = new BatBreakerContext();
            var players = db.Players.ToList();

            List<StatModels.BatterStats> stats = new List<StatModels.BatterStats>();
            foreach (Player p in players)
            {
                stats.Add(GetBatterStats(p));
            }

            return View(stats.OrderByDescending(s => s.Avg).ToList());
        }

        public ActionResult HeadsUpStats(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            return View(db.AtBats.Where(ab => ab.Id.Equals(id)).FirstOrDefault());
        }

        public ActionResult GetPlayerStats(int id){
            BatBreakerContext db = new BatBreakerContext();
            Player p = db.Players.Where(pl => pl.Id.Equals(id)).FirstOrDefault();

            if (p.positionId == 1)
            {
                return View("PitcherStats", GetPitcherStats(p));
            }
            else
            {
                return View("BatterStats", GetBatterStats(p));
            }
        }

        private StatModels.PitcherStats GetPitcherStats(Player p)
        {
            StatModels.PitcherStats stats = new StatModels.PitcherStats();
            stats.BattingStats.player = p;
            stats.HAgainst = p.AtBatsPitched.Where(ab => ab.resultId > 3).Count();
            stats.HRAgainst = p.AtBatsPitched.Where(ab => ab.resultId == AtBatResultNum.HomeRun).Count();
            if (p.AtBatsPitched.Where(ab => ab.resultId != AtBatResultNum.Walk).Count() > 0)
            {
                stats.AvgAgainst = stats.HAgainst.ToDecimal() / p.AtBatsPitched.Where(ab => ab.resultId != AtBatResultNum.Walk).Count().ToDecimal();
            }
            else
            {
                stats.AvgAgainst = 0M;
            }
            stats.BB = p.AtBatsPitched.Where(ab => ab.resultId == AtBatResultNum.Walk).Count();
            stats.K = p.AtBatsPitched.Where(ab => ab.resultId == AtBatResultNum.Strikeout).Count();
            string IP = "";
            IP = Math.Floor(p.AtBatsPitched.Where(ab => ab.resultId == AtBatResultNum.Strikeout || ab.resultId == AtBatResultNum.Out).Count().ToDecimal() / 3M).ToString();
            if (p.AtBatsPitched.Where(ab => ab.resultId == AtBatResultNum.Strikeout || ab.resultId == AtBatResultNum.Out).Count() % 3 > 0)
            {
                IP += "." + (p.AtBatsPitched.Where(ab => ab.resultId == AtBatResultNum.Strikeout || ab.resultId == AtBatResultNum.Out).Count() % 3).ToString();
            }
            stats.IP = IP.ToDecimal();
            if (stats.IP > 0M)
            {
                decimal mathIP = IP.ToDecimal();
                int floorIP = Math.Floor(double.Parse(IP.ToString())).ToInt();
                if ((mathIP - floorIP.ToDecimal()) > 0){
                    mathIP += (mathIP - floorIP.ToDecimal()) * .3333M;
                }
                stats.ERA = p.RunsScoredAgainst.Count.ToDecimal() / (mathIP / 9M);
            }
            else
            {
                stats.ERA = 0M;
            }
            stats.BattingStats = GetBatterStats(p);
            return stats;
        }

        private StatModels.BatterStats GetBatterStats(Player p) {
            StatModels.BatterStats stats = new StatModels.BatterStats();
            stats.player = p;
            stats.ABs = p.AtBatsHit.Where(ab => ab.resultId != AtBatResultNum.Walk).Count();
            stats.Hits = p.AtBatsHit.Where(ab => ab.resultId > 3).Count();
            stats.BB = p.AtBatsHit.Where(ab => ab.resultId == AtBatResultNum.Walk).Count();
            stats.Doubles = p.AtBatsHit.Where(ab => ab.resultId == AtBatResultNum.Double).Count();
            stats.Singles = p.AtBatsHit.Where(ab => ab.resultId == AtBatResultNum.Single).Count();
            stats.Triples = p.AtBatsHit.Where(ab => ab.resultId == AtBatResultNum.Triple).Count();
            stats.HR = p.AtBatsHit.Where(ab => ab.resultId == AtBatResultNum.HomeRun).Count();
            stats.K = p.AtBatsHit.Where(ab => ab.resultId == AtBatResultNum.Strikeout).Count();
            stats.R = p.RunsScoreds.Count;
            stats.RBI = p.AtBatsHit.Sum(r => r.RunsScoreds.Count);

            if (stats.ABs + stats.BB > 0)
            {
                stats.OBP = (stats.Hits + stats.BB).ToDecimal() / (stats.ABs + stats.BB).ToDecimal();
            }
            else
            {
                stats.OBP = 0M;
            }
            stats.TB = (stats.Singles) + (stats.Doubles * 2) + (stats.Triples * 3) + (stats.HR * 4);
            if (stats.ABs > 0)
            {
                stats.Avg = stats.Hits.ToDecimal() / stats.ABs.ToDecimal();
                stats.SLG = stats.TB.ToDecimal() / stats.ABs.ToDecimal();
            }
            else
            {
                stats.Avg = 0M;
                stats.SLG = 0M;
            }
            stats.OPS = stats.OBP + stats.SLG;

            return stats;
        }

        public ActionResult PlayerAttributes(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            Player p = db.Players.Where(pl => pl.Id.Equals(id)).FirstOrDefault();
            return View(p);
        }

        public ActionResult GetAllTeams()
        {
            BatBreakerContext db = new BatBreakerContext();
            List<Team> teams = db.Teams.ToList();

            return View(teams);
        }

        public ActionResult TeamGames(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            List<Game> games = db.Games.Where(g => g.homeTeamId.Equals(id) || g.awayTeamId.Equals(id)).ToList();

            ViewData["teamId"] = id;

            return View(games);
        }

        public ActionResult Game(Guid id)
        {
            BatBreakerContext db = new BatBreakerContext();
            Game game = db.Games.Where(g => g.guId.Equals(id)).FirstOrDefault();

            return View(game);
        }
    }
}
