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
    public class AtBatController : Controller
    {
        

        public JsonResult New(Guid gameId, int AtFirst = 0, int AtSecond = 0, int AtThird = 0)
        {
            return Json(DoNewAtBat(gameId, AtFirst, AtSecond, AtThird), JsonRequestBehavior.AllowGet);
        }

        public ABReturn DoNewAtBat(Guid gameId, int AtFirst, int AtSecond, int AtThird)
        {
            BatBreakerContext db = new BatBreakerContext();
            Game game = db.Games.Where(g => g.guId.Equals(gameId)).First();

            if (game != null && game.AtBats.Count > 0 && game.AtBats.Last().resultId == 0)
            {
                AtBat exAb = game.AtBats.Last();
                ABReturn exOutput = new ABReturn();
                exOutput.Id = exAb.Id;
                exOutput.IsUserPlayer = exAb.Batter.userId == User.plainUserId();
                exOutput.batterId = exAb.batterId;
                exOutput.HomeAtBat = exAb.Batter.teamId == exAb.Game.homeTeamId;
                exOutput.BatterName = exAb.Batter.FirstName + " " + exAb.Batter.LastName;

                AtFirst = exAb.AtFirst;
                AtSecond = exAb.AtSecond;
                AtThird = exAb.AtThird;

                if (AtFirst == 0)
                {
                    if (AtSecond == 0)
                    {
                        if (AtThird == 0)
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/basesempty.gif\">";
                        }
                        else
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/third.gif\">";
                        }
                    }
                    else
                    {
                        if (AtThird == 0)
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/second.gif\">";
                        }
                        else
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/secondthird.gif\">";
                        }
                    }
                }
                else
                {
                    if (AtSecond == 0)
                    {
                        if (AtThird == 0)
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/first.gif\">";
                        }
                        else
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/firstthird.gif\">";
                        }
                    }
                    else
                    {
                        if (AtThird == 0)
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/firstsecond.gif\">";
                        }
                        else
                        {
                            exOutput.BasesImage = "<img src=\"/Content/bases/loaded.gif\">";
                        }
                    }
                }

                return exOutput;
            }

            Innings inning = game.Innings.Last();
            if (game.Innings.Last().Outs == 3)
            {
                inning = new Innings();
                inning.gameId = game.Id;
                if (game.Innings.Last().TopOfInning)
                {
                    inning.Number = game.Innings.Last().Number;
                    inning.TopOfInning = false;
                }
                else
                {
                    inning.Number = game.Innings.Last().Number + 1;
                    inning.TopOfInning = true;
                }
                inning.StartDate = DateTime.Now;

                db.Innings.Add(inning);
                db.SaveChanges();
            }

            int BatterId = 0;
            int PitcherId = 0;

            if (inning.TopOfInning)
            {
                PitcherId = game.Lineups.Where(l => l.positionId.Equals(1) && l.teamId.Equals(game.homeTeamId)).First().playerId;

            }
            else
            {
                PitcherId = game.Lineups.Where(l => l.positionId.Equals(1) && l.teamId.Equals(game.awayTeamId)).First().playerId;
            }

            AtBat last = inning.AtBats.LastOrDefault();
            if (last != null)
            {
                Lineup lastLineup = game.Lineups.Where(l => l.playerId.Equals(last.batterId)).FirstOrDefault();

                int BattingOrder = lastLineup.BattingOrder + 1;
                if (BattingOrder > 9) BattingOrder = 1;

                BatterId = game.Lineups.Where(l => l.teamId.Equals(lastLineup.teamId) && l.BattingOrder.Equals(BattingOrder)).FirstOrDefault().playerId;
            }
            else
            {
                Innings lastInning = game.Innings.Where(ii => ii.TopOfInning == inning.TopOfInning && ii.Id != inning.Id).LastOrDefault();
                if (lastInning != null)
                {
                    last = lastInning.AtBats.LastOrDefault();
                    Lineup lastLineup = game.Lineups.Where(l => l.playerId.Equals(last.batterId)).FirstOrDefault();
                    if (lastLineup != null)
                    {
                        int BattingOrder = lastLineup.BattingOrder + 1;
                        if (BattingOrder > 9) BattingOrder = 1;

                        BatterId = game.Lineups.Where(l => l.teamId.Equals(lastLineup.teamId) && l.BattingOrder.Equals(BattingOrder)).FirstOrDefault().playerId;
                    }
                }
                else
                {
                    if (inning.TopOfInning)
                    {
                        BatterId = game.Lineups.Where(l => l.teamId.Equals(game.awayTeamId) && l.BattingOrder.Equals(1)).First().playerId;
                    }
                    else
                    {
                        BatterId = game.Lineups.Where(l => l.teamId.Equals(game.homeTeamId) && l.BattingOrder.Equals(1)).First().playerId;
                    }
                }
            }

            AtBat lastAB = game.AtBats.LastOrDefault();

            AtBat ab = db.AtBats.Create();
            ab.gameId = game.Id;
            ab.userId = User.plainUserId();
            ab.inningId = inning.Id;
            ab.pitcherId = PitcherId;
            ab.batterId = BatterId;
            ab.Outs = 0;
            ab.AtFirst = 0;
            ab.AtSecond = 0;
            ab.AtThird = 0;
            ab.resultId = 0;
            ab.resultTypeId = 0;
            ab.Bases = 0;
            ab.Runs = 0;
            ab.EarnedRuns = 0;
            ab.CreateDate = DateTime.Now;
            ab.AtFirst = AtFirst;
            ab.AtSecond = AtSecond;
            ab.AtThird = AtThird;

            db.AtBats.Add(ab);
            db.SaveChanges();

            ABReturn output = new ABReturn();
            output.Id = ab.Id;
            output.IsUserPlayer = ab.Batter.userId == User.plainUserId();
            output.batterId = ab.batterId;
            output.HomeAtBat = ab.Batter.teamId == ab.Game.homeTeamId;
            output.BatterName = ab.Batter.FirstName + " " + ab.Batter.LastName;

            if (AtFirst == 0)
            {
                if (AtSecond == 0)
                {
                    if (AtThird == 0)
                    {
                        output.BasesImage = "<img src=\"/Content/bases/basesempty.gif\">";
                    }
                    else
                    {
                        output.BasesImage = "<img src=\"/Content/bases/third.gif\">";
                    }
                }
                else
                {
                    if (AtThird == 0)
                    {
                        output.BasesImage = "<img src=\"/Content/bases/second.gif\">";
                    }
                    else
                    {
                        output.BasesImage = "<img src=\"/Content/bases/secondthird.gif\">";
                    }
                }
            }
            else
            {
                if (AtSecond == 0)
                {
                    if (AtThird == 0)
                    {
                        output.BasesImage = "<img src=\"/Content/bases/first.gif\">";
                    }
                    else
                    {
                        output.BasesImage = "<img src=\"/Content/bases/firstthird.gif\">";
                    }
                }
                else
                {
                    if (AtThird == 0)
                    {
                        output.BasesImage = "<img src=\"/Content/bases/firstsecond.gif\">";
                    }
                    else
                    {
                        output.BasesImage = "<img src=\"/Content/bases/loaded.gif\">";
                    }
                }
            }

            if (lastAB != null)
            {
                string BatterStats = "";
                var playerABs = ab.Game.AtBats.Where(a => a.batterId.Equals(lastAB.batterId) && a.resultId > 0);
                if (playerABs.Count() > 0)
                {
                    BatterStats = playerABs.Where(a => a.resultId > 3).Count().ToString() + "-" + playerABs.Where(a => a.resultId != AtBatResultNum.Walk).Count();

                    int Ks = playerABs.Where(a => a.resultId == AtBatResultNum.Strikeout).Count();
                    if (Ks > 1)
                    {
                        BatterStats += ", " + Ks.ToString() + " Ks";
                    }
                    else if (Ks == 1)
                    {
                        BatterStats += ", K";
                    }

                    int Walks = playerABs.Where(a => a.resultId == AtBatResultNum.Walk).Count();
                    if (Walks > 1)
                    {
                        BatterStats += ", " + Walks.ToString() + " BB";
                    }
                    else if (Walks == 1)
                    {
                        BatterStats += ", BB";
                    }

                    int Singles = playerABs.Where(a => a.resultId == AtBatResultNum.Single).Count();
                    if (Singles > 1)
                    {
                        BatterStats += ", " + Singles.ToString() + " Singles";
                    }
                    else if (Singles == 1)
                    {
                        BatterStats += ", Single";
                    }

                    int Doubles = playerABs.Where(a => a.resultId == AtBatResultNum.Double).Count();
                    if (Doubles > 1)
                    {
                        BatterStats += ", " + Doubles.ToString() + " Doubles";
                    }
                    else if (Doubles == 1)
                    {
                        BatterStats += ", Double";
                    }

                    int Triples = playerABs.Where(a => a.resultId == AtBatResultNum.Triple).Count();
                    if (Triples > 1)
                    {
                        BatterStats += ", " + Triples.ToString() + " Triples";
                    }
                    else if (Triples == 1)
                    {
                        BatterStats += ", Triple";
                    }

                    int HR = playerABs.Where(a => a.resultId == AtBatResultNum.HomeRun).Count();
                    if (HR > 1)
                    {
                        BatterStats += ", " + HR.ToString() + " HR";
                    }
                    else if (HR == 1)
                    {
                        BatterStats += ", HR";
                    }

                    int RBI = playerABs.Sum(a => a.RunsScoreds.Count);
                    if (RBI > 1)
                    {
                        BatterStats += ", " + RBI.ToString() + " RBI";
                    }
                    else if (RBI == 1)
                    {
                        BatterStats += ", RBI";
                    }
                }
                output.lastBatterId = lastAB.batterId;
                output.BatterGameStats = BatterStats;
            }
            return output;
        }

        public class ABReturn
        {
            public int Id { get; set; }
            public bool IsUserPlayer { get; set; }
            public int batterId { get; set; }
            public bool HomeAtBat { get; set; }
            public string BasesImage { get; set; }
            public string BatterName { get; set; }
            public int lastBatterId { get; set; }
            public string BatterGameStats { get; set; }
        }

    }
}
