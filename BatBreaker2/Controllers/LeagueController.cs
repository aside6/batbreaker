using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatBreaker2.Models;
using CommonLibrary;
using System.Globalization;
using BatBreaker2.Common;

namespace BatBreaker.Controllers
{
    public class LeagueController : Controller
    {
        Random rnd = new Random();
        List<GameSchedule> used = new List<GameSchedule>();

        public ActionResult Generate()
        {
            BatBreakerContext db = new BatBreakerContext();
            League league = new League();
            league.CreateDate = DateTime.Now;
            league.Title = (db.Leagues.Count() + 1).ToString();
            league.IsOpen = true;

            db.Leagues.Add(league);
            db.SaveChanges();

            for (int i = 0; i < 6; i++)
            {
                Division div = new Division();
                div.leagueId = league.Id;
                div.Title = "Division " + (i + 1).ToString();

                db.Divisions.Add(div);
                db.SaveChanges();
            }

            int FCount = db.FirstNames.Count();
            int LCount = db.LastNames.Count();
            int CCount = db.Cities.Count();
            int TCount = db.TeamNames.Count();
            int PCount = db.Pitches.Count();

            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            List<int> teamIds = new List<int>();

            List<string> arm = new List<String>();
            arm.Add("L");
            arm.Add("R");
            Random rnd = new Random();
            for (int i = 0; i < 30; i++)
            {
                string division = "0";
                if (i < 5)
                {
                    division = "1";
                }
                else if (i < 10)
                {
                    division = "2";
                }
                else if (i < 15)
                {
                    division = "3";
                }
                else if (i < 20)
                {
                    division = "4";
                }
                else if (i < 25)
                {
                    division = "5";
                }
                else
                {
                    division = "6";
                }
                Team team = db.Teams.Create();
                team.leagueId = league.Id;
                team.userId = 0;
                team.City = db.Cities.ToList()[rnd.Next(CCount)].CityName;
                team.Title = db.TeamNames.ToList()[rnd.Next(TCount)].TeamName1;
                team.divisionId = db.Divisions.Where(d => d.leagueId.Equals(league.Id) && d.Title.Equals("Division " + division)).FirstOrDefault().Id;

                db.Teams.Add(team);
                db.SaveChanges();

                teamIds.Add(team.Id);

                for (int iP = 0; iP < 10; iP++)
                {
                    Player p = db.Players.Create();
                    p.teamId = team.Id;
                    p.FirstName = myTI.ToTitleCase(db.FirstNames.ToList()[rnd.Next(FCount)].FirstName1.ToLower());
                    p.LastName = myTI.ToTitleCase(db.LastNames.ToList()[rnd.Next(LCount)].LastName1.ToLower());
                    p.positionId = 1;
                    p.Throwing = arm[rnd.Next(2)];
                    p.Batting = arm[rnd.Next(2)];
                    p.userId = 0;
                    p.CreateDate = DateTime.Now;

                    db.Players.Add(p);
                    db.SaveChanges();

                    List<int> ptypes = new List<int>();

                    int pitches = rnd.Next(2, 4);
                    int pnum = 0;

                    while (pnum < pitches)
                    {
                        int ptype = db.Pitches.ToList()[rnd.Next(PCount)].Id;
                        if (!ptypes.Contains(ptype))
                        {
                            PlayerPitch pp = db.PlayerPitches.Create();
                            pp.playerId = p.Id;
                            pp.pitchId = ptype;
                            pp.Speed = rnd.Next(60, 92);
                            pp.Movement = rnd.Next(60, 92);
                            pp.Control = rnd.Next(60, 92);

                            db.PlayerPitches.Add(pp);
                            db.SaveChanges();
                            ptypes.Add(ptype);
                            pnum++;
                        }
                    }

                    PlayerAttribute pa = db.PlayerAttributes.Create();
                    pa.ArmAccuracy = rnd.Next(60, 92);
                    pa.ArmPower = rnd.Next(60, 92);
                    pa.BattingClutch = rnd.Next(25, 45);
                    pa.BB9 = rnd.Next(60, 92);
                    pa.Blocking = rnd.Next(0, 30);
                    pa.BRAbility = rnd.Next(25, 50);
                    pa.BRAgressiveness = rnd.Next(25, 50);
                    pa.Confidence = rnd.Next(60, 92);
                    pa.ContactVsLeft = rnd.Next(25, 45);
                    pa.ContactVsRight = rnd.Next(25, 45);
                    pa.Durability = rnd.Next(60, 92);
                    pa.Energy = 99;
                    pa.Fielding = rnd.Next(10, 70);
                    pa.GameCalling = rnd.Next(35, 70);
                    pa.H9 = rnd.Next(60, 92);
                    pa.HR9 = rnd.Next(60, 92);
                    pa.K9 = rnd.Next(60, 92);
                    pa.PitchingClutch = rnd.Next(60, 92);
                    pa.PowerVsLeft = rnd.Next(25, 50);
                    pa.PowerVsRight = rnd.Next(25, 50);
                    pa.PlateDiscipline = rnd.Next(20, 60);
                    pa.PlateVision = rnd.Next(20, 60);
                    pa.Reaction = rnd.Next(35, 65);
                    pa.Speed = rnd.Next(35, 65);
                    pa.Stamina = rnd.Next(60, 92);
                    pa.playerId = p.Id;

                    db.PlayerAttributes.Add(pa);
                    db.SaveChanges();
                }

                CreatePlayer(team.Id, 2);
                CreatePlayer(team.Id, 2);
                CreatePlayer(team.Id, 3);
                CreatePlayer(team.Id, 4);
                CreatePlayer(team.Id, 5);
                CreatePlayer(team.Id, 6);
                CreatePlayer(team.Id, 7);
                CreatePlayer(team.Id, 8);
                CreatePlayer(team.Id, 9);
                CreatePlayer(team.Id, 3);
                CreatePlayer(team.Id, 4);
                CreatePlayer(team.Id, 6);
                CreatePlayer(team.Id, 5);
                CreatePlayer(team.Id, 8);
                CreatePlayer(team.Id, 9);

            }

            DoSchedule(teamIds, 1, league.Id);
            DoSchedule(teamIds, 2, league.Id);
            DoSchedule(teamIds, 3, league.Id);
            DoSchedule(teamIds, 4, league.Id);
            DoSchedule(teamIds, 5, league.Id);
            DoSchedule(teamIds, 6, league.Id);
            DoSchedule(teamIds, 7, league.Id);
            DoSchedule(teamIds, 8, league.Id);
            DoSchedule(teamIds, 9, league.Id);
            DoSchedule(teamIds, 10, league.Id);
            DoSchedule(teamIds, 11, league.Id);
            DoSchedule(teamIds, 12, league.Id);
            DoSchedule(teamIds, 13, league.Id);
            DoSchedule(teamIds, 14, league.Id);
            DoSchedule(teamIds, 15, league.Id);
            DoSchedule(teamIds, 16, league.Id);
            DoSchedule(teamIds, 17, league.Id);
            DoSchedule(teamIds, 18, league.Id);
            DoSchedule(teamIds, 19, league.Id);
            DoSchedule(teamIds, 20, league.Id);
            DoSchedule(teamIds, 21, league.Id);
            DoSchedule(teamIds, 22, league.Id);
            DoSchedule(teamIds, 23, league.Id);
            DoSchedule(teamIds, 24, league.Id);
            DoSchedule(teamIds, 25, league.Id);
            DoSchedule(teamIds, 26, league.Id);
            DoSchedule(teamIds, 27, league.Id);
            DoSchedule(teamIds, 28, league.Id);
            DoSchedule(teamIds, 29, league.Id);
            DoSchedule(teamIds, -1, league.Id);
            DoSchedule(teamIds, -2, league.Id);
            DoSchedule(teamIds, -3, league.Id);
            DoSchedule(teamIds, -4, league.Id);
            DoSchedule(teamIds, -5, league.Id);
            DoSchedule(teamIds, -6, league.Id);
            DoSchedule(teamIds, -7, league.Id);
            DoSchedule(teamIds, -8, league.Id);
            DoSchedule(teamIds, -9, league.Id);
            DoSchedule(teamIds, -10, league.Id);
            DoSchedule(teamIds, -11, league.Id);
            DoSchedule(teamIds, -12, league.Id);
            DoSchedule(teamIds, -13, league.Id);
            DoSchedule(teamIds, -14, league.Id);
            DoSchedule(teamIds, -15, league.Id);
            DoSchedule(teamIds, -16, league.Id);
            DoSchedule(teamIds, -17, league.Id);
            DoSchedule(teamIds, -18, league.Id);
            DoSchedule(teamIds, -19, league.Id);
            DoSchedule(teamIds, -20, league.Id);
            DoSchedule(teamIds, -21, league.Id);
            DoSchedule(teamIds, -22, league.Id);
            DoSchedule(teamIds, -23, league.Id);
            DoSchedule(teamIds, -24, league.Id);
            DoSchedule(teamIds, -25, league.Id);
            DoSchedule(teamIds, -26, league.Id);
            DoSchedule(teamIds, -27, league.Id);
            DoSchedule(teamIds, -28, league.Id);
            DoSchedule(teamIds, -29, league.Id);

            return Content("League Generated");
        }

        private void DoSchedule(List<int> TeamIds, int tApart, int leagueId)
        {
            BatBreakerContext db = new BatBreakerContext();
            if (tApart > 0)
            {
                for (int i = 0; i < TeamIds.Count; i++)
                {
                    int hId = TeamIds[i];
                    int iNext = i + tApart;
                    if (iNext > TeamIds.Count - 1)
                    {
                        iNext = iNext - TeamIds.Count;
                    }
                    int aId = TeamIds[iNext];
                    GameSchedule s = new GameSchedule();
                    s.homeId = hId;
                    s.awayId = aId;

                    if (used.Where(u => u.homeId == hId && u.awayId == aId).Count() < 1 && hId != aId)
                    {
                        Schedule schedule = db.Schedules.Create();
                        schedule.homeTeamId = hId;
                        schedule.awayTeamId = aId;
                        schedule.leagueId = leagueId;
                        schedule.SeasonNum = 1;
                        db.Schedules.Add(schedule);

                        schedule = db.Schedules.Create();
                        schedule.homeTeamId = hId;
                        schedule.awayTeamId = aId;
                        schedule.leagueId = leagueId;
                        schedule.SeasonNum = 1;
                        db.Schedules.Add(schedule);

                        schedule = db.Schedules.Create();
                        schedule.homeTeamId = hId;
                        schedule.awayTeamId = aId;
                        schedule.leagueId = leagueId;
                        schedule.SeasonNum = 1;
                        db.Schedules.Add(schedule);
                        db.SaveChanges();
                        used.Add(s);
                    }
                }
            }
            else
            {
                for (int i = 29; i >= 0; i--)
                {
                    int hId = TeamIds[i];
                    int iNext = i + tApart;
                    if (iNext > 29)
                    {
                        iNext = iNext - 29;
                    }
                    if (iNext < 0)
                    {
                        iNext = iNext + 29;
                    }
                    int aId = TeamIds[iNext];
                    GameSchedule s = new GameSchedule();
                    s.homeId = hId;
                    s.awayId = aId;

                    if (used.Where(u => u.homeId == hId && u.awayId == aId).Count() < 1 && hId != aId)
                    {
                        Schedule schedule = db.Schedules.Create();
                        schedule.homeTeamId = hId;
                        schedule.awayTeamId = aId;
                        schedule.leagueId = leagueId;
                        schedule.SeasonNum = 1;
                        db.Schedules.Add(schedule);

                        schedule = db.Schedules.Create();
                        schedule.homeTeamId = hId;
                        schedule.awayTeamId = aId;
                        schedule.leagueId = leagueId;
                        schedule.SeasonNum = 1;
                        db.Schedules.Add(schedule);

                        schedule = db.Schedules.Create();
                        schedule.homeTeamId = hId;
                        schedule.awayTeamId = aId;
                        schedule.leagueId = leagueId;
                        schedule.SeasonNum = 1;
                        db.Schedules.Add(schedule);
                        db.SaveChanges();
                        used.Add(s);
                    }
                }
            }
        }

        private class GameSchedule
        {
            public int homeId { get; set; }
            public int awayId { get; set; }
        }

        private void CreatePlayer(int teamId, int positionId)
        {
            BatBreakerContext db = new BatBreakerContext();
            int FCount = db.FirstNames.Count();
            int LCount = db.LastNames.Count();
            int CCount = db.Cities.Count();
            int TCount = db.TeamNames.Count();
            Random rnd = new Random();

            List<string> arm = new List<String>();
            arm.Add("L");
            arm.Add("R");

            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            Player p = db.Players.Create();
            p.teamId = teamId;
            p.FirstName = myTI.ToTitleCase(db.FirstNames.ToList()[rnd.Next(FCount)].FirstName1.ToLower());
            p.LastName = myTI.ToTitleCase(db.LastNames.ToList()[rnd.Next(LCount)].LastName1.ToLower());
            p.positionId = positionId;
            p.Throwing = arm[rnd.Next(2)];
            p.Batting = arm[rnd.Next(2)];
            p.userId = 0;
            p.CreateDate = DateTime.Now;

            db.Players.Add(p);
            db.SaveChanges();

            PlayerAttribute pa = db.PlayerAttributes.Create();
            pa.ArmAccuracy = rnd.Next(62, 92);
            pa.ArmPower = rnd.Next(62, 92);
            pa.BattingClutch = rnd.Next(62, 92);
            pa.BB9 = 0;
            pa.Blocking = rnd.Next(62, 92);
            pa.BRAbility = rnd.Next(62, 92);
            pa.BRAgressiveness = rnd.Next(62, 92);
            pa.Confidence = rnd.Next(62, 92);
            pa.ContactVsLeft = rnd.Next(62, 92);
            pa.ContactVsRight = rnd.Next(62, 92);
            pa.Durability = rnd.Next(62, 92);
            pa.Energy = 99;
            pa.Fielding = rnd.Next(62, 92);
            pa.GameCalling = rnd.Next(62, 92);
            pa.H9 = 0;
            pa.HR9 = 0;
            pa.K9 = 0;
            pa.PitchingClutch = 0;
            pa.PlateDiscipline = rnd.Next(62, 92);
            pa.PlateVision = rnd.Next(62, 92);
            pa.Reaction = rnd.Next(62, 92);
            pa.Speed = rnd.Next(62, 92);
            pa.Stamina = rnd.Next(62, 92);
            pa.playerId = p.Id;
            pa.PowerVsLeft = rnd.Next(42, 86);
            pa.PowerVsRight = rnd.Next(42, 86);

            db.PlayerAttributes.Add(pa);
            db.SaveChanges();
        }

        public ActionResult SimulateSeason(int id)
        {
            BatBreakerContext db = new BatBreakerContext();
            var schedule = db.Schedules.Where(s => s.Games.FirstOrDefault() == null || !s.Games.FirstOrDefault().EndDate.HasValue);
            foreach (Schedule s in schedule)
            {
                Guid gameId = System.Guid.NewGuid();
                if (s.Games.FirstOrDefault() != null)
                {
                    gameId = s.Games.FirstOrDefault().guId;

                }
                else
                {
                    GameController gc = new GameController();
                    gameId = new Guid(gc.DoNewGame(s.Id));
                }
                Game game = db.Games.Where(g => g.guId.Equals(gameId)).FirstOrDefault();
                Innings inning = game.Innings.Last();
                bool GameOver = false;
                int AtFirst = 0;
                int AtSecond = 0;
                int AtThird = 0;
                int Outs = 0;
                while (!game.EndDate.HasValue && !GameOver)
                {
                    while (Outs < 3)
                    {
                        AtBatController ac = new AtBatController();
                        AtBatController.ABReturn ab = ac.DoNewAtBat(gameId, AtFirst, AtSecond, AtThird);
                        bool AbOver = false;
                        while (!AbOver)
                        {
                            PitchController pc = new PitchController();
                            BatBreaker2.Common.PitchResult p = pc.DoThrowPitch(gameId, ab.Id, "Standard", "Batter", rnd);
                            AbOver = p.Over;
                            GameOver = p.GameOver;
                            AtFirst = p.OnFirst;
                            AtSecond = p.OnSecond;
                            AtThird = p.OnThird;
                            Outs = p.Outs;
                        }
                    }
                    if (!game.EndDate.HasValue && !GameOver)
                    {
                        InningController ic = new InningController();
                        ic.DoNewInning(gameId);
                        Outs = 0;
                        AtFirst = 0;
                        AtThird = 0;
                        AtSecond = 0;
                    }
                    
                }
            }

            return Content("Season over");
        }

    }
}
