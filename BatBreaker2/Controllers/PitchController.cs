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
    public class PitchController : Controller
    {        
        public string Scored = "";
        public int Score = 0;
        public int Points = 0;
        public int PitchResult = 0;
        public int AtBatResult = 0;
        public string AbResultString = "";
        public const decimal FlyD = .6M;
        public const decimal PopD = .2M;
        public const decimal LineD = .78M;
        public const decimal GroundD = .61M;
        public const decimal ChopperD = .2M;
        public const decimal PGroundD = 2M;
        Random rnd = new Random();

        private List<Player> _batters;
        public List<Player> Batters
        {
            get
            {
                if (_batters != null)
                {
                    BatBreakerContext db = new BatBreakerContext();
                    _batters = db.Players.Include("PlayerAttributes").Where(p => p.positionId != 9).ToList();
                }
                return _batters;
            }
        }

        public JsonResult Throw(Guid gameId, int abId, string style, string player)
        {
            return Json(DoThrowPitch(gameId, abId, style, player), JsonRequestBehavior.AllowGet);
        }

        public PitchResult DoThrowPitch(Guid gameId, int abId, string style, string player, Random newRnd = null)
        {
            if (newRnd != null)
            {
                rnd = newRnd;
            }

            BatBreakerContext db = new BatBreakerContext();
            AtBat ab = db.AtBats.Where(a => a.Id.Equals(abId) && a.Game.guId.Equals(gameId)).FirstOrDefault();
            if (ab == null || ab.resultId > 0)
            {
                PitchResult abOver = new PitchResult();
                abOver.Over = true;
                return abOver;
            }     

            decimal ConMissMod = 1M;
            decimal PowerMod = 1M;
            decimal SwingMod = 1M;
            decimal diffMod = 1M;

            int userId = User.plainUserId();
            if (ab.Batter.userId == userId)
            {
                switch (style)
                {
                    case "ShortenUp":
                        ConMissMod = .7M;
                        PowerMod = .3M;
                        SwingMod = 1.2M;
                        diffMod = 1.15M;
                        break;
                    case "Power":
                        ConMissMod = 1.3M;
                        PowerMod = 1.4M;
                        SwingMod = 1.2M;
                        diffMod = .85M;
                        break;
                    case "TakePitch":
                        SwingMod = 0M;
                        break;
                }
            }

            string resultString = "";

            PitchesThrown thrown = db.PitchesThrowns.Create();
            thrown.abId = abId;
            thrown.batterId = ab.batterId;
            thrown.pitcherId = ab.pitcherId;
            thrown.CreateDate = DateTime.Now;

            int Balls = ab.PitchesThrowns.Where(pt => pt.resultTypeId == 2).Count();
            int Strikes = ab.PitchesThrowns.Where(pt => pt.resultTypeId == 1 || pt.resultTypeId == 3 || pt.resultTypeId == 4).Count();
            if (Strikes > 2)
                Strikes = 2;

            PlayerPitch playerPitch = ab.Pitcher.PlayerPitches.ToList()[rnd.Next(0, ab.Pitcher.PlayerPitches.Count)];

            thrown.pitchId = playerPitch.pitchId;

            resultString += "<br>" + playerPitch.Pitch.Title;

            bool Strike = false;

            PitchPosition pitchPosition = GetPitchPosition(playerPitch, ab.Pitcher, 1M);
            if (pitchPosition.X >= 0 && pitchPosition.X <= 90 && pitchPosition.Y >= 0 && pitchPosition.Y <= 120)
            {
                Strike = true;
            }

            thrown.X = pitchPosition.X;
            thrown.Y = pitchPosition.Y;
            thrown.startX = pitchPosition.startX;
            thrown.startY = pitchPosition.startY;

            //resultString += "<br>" + Strike.ToString();

            PitchVelocity velocity = GetPitchVelocity(playerPitch, 0);

            thrown.Speed = velocity.Speed;

            resultString += "<br>" + velocity.Speed + " mph";

            bool Swing = SwingBatter(pitchPosition, ab.Batter, ab, SwingMod);

            SwingResult sResult;

            if (Swing)
            {
                sResult = BatterContact(pitchPosition, velocity, ab, playerPitch, ConMissMod, PowerMod);
                resultString += "<br>" + sResult.Result;
                if (sResult.ToPosition > 0)
                {
                    resultString += " to " + sResult.ToPosition;
                    thrown.Power = sResult.Power;
                    thrown.PlayDifficulty = sResult.PlayDifficulty * diffMod;
                }
            }
            else
            {
                sResult = null;
            }

            //PlayerAttribute pA = ab.Pitcher.PlayerAttributes.First();
            PlayerAttribute bA = ab.Batter.PlayerAttributes.First();

            decimal mx = 100M;

            bool AtBatOver = false;

            if (Swing && sResult.Result != "Miss" && sResult.Result != "Foul")
            {
                PitchResult = PitchResultNum.InPlay;
                if (sResult.Result == "Fly" && sResult.Power > 100)
                {
                    AtBatOver = true;
                    Points += 20;
                    ab = MoveBaseRunners(ab, 4, sResult.ResultId, sResult.ToPosition);
                    AtBatResult = AtBatResultNum.HomeRun;
                    AbResultString = "Home Run";
                    resultString += "<br>Home Run";
                    ScoreRun(ab, ab.Batter);
                }
                else
                {
                    if (rnd.Next(100) > (sResult.PlayDifficulty * 100M))
                    {
                        AtBatOver = true;
                        Points -= 1;
                        ab.Innings.Outs++;
                        DefensivePlay d = new DefensivePlay();
                        d.Result = 1;
                        d.playerId = Defender(ab.Game, ab.Batter.teamId == ab.Game.homeTeamId, sResult.ToPosition).playerId;
                        d.CreateDate = DateTime.Now;
                        ab.DefensivePlays.Add(d);
                        db.SaveChanges();

                        int dPoints = (15M * sResult.PlayDifficulty).ToInt();

                        int UserId = User.plainUserId();
                        if (d.Player.userId > 0)
                        {
                            if (d.Player.userId == UserId)
                            {
                                d.Player.Points += dPoints;
                            }
                            else
                            {
                                d.Player.Points += dPoints / 2;
                            }
                        }

                        AtBatResult = AtBatResultNum.Out;
                        resultString += "<br>Out";
                        AbResultString = "Out";

                        if (ab.Innings.Outs < 3)
                        {
                            if (sResult.SecondaryPosition > 0)
                            {
                                ab = MoveBaseRunners(ab, 0, sResult.ResultId, sResult.SecondaryPosition);
                            }
                            else
                            {
                                ab = MoveBaseRunners(ab, 0, sResult.ResultId, sResult.ToPosition);
                            }
                        }
                    }
                    else
                    {
                        BattingResultType type = db.BattingResultTypes.Where(b => b.Title.Equals(sResult.Result)).FirstOrDefault();

                        int OnFirst = ab.AtFirst;
                        int OnSecond = ab.AtSecond;
                        int OnThird = ab.AtThird;

                        string HitType = "";

                        thrown.resultTypeId = 5;
                        AbResult result = new AbResult();
                        result.Single = (type.SingleRatio * mx).ToInt();
                        result.Double = (type.DoubleRatio * mx * bA.Speed.valModifier() * bA.BRAbility.valModifier()).ToInt();
                        result.Triple = (type.TripleRatio * mx * bA.Speed.valModifier() * bA.BRAbility.valModifier() * bA.BRAgressiveness.valModifier()).ToInt();

                        int ContactTotal = result.Single + result.Double + result.Triple + result.HomeRun + result.Out + result.OutOfPlay;

                        List<string> ContactRandomizer = new List<string>();
                        ContactRandomizer.AddRange(RandomAdd("Single", result.Single));
                        ContactRandomizer.AddRange(RandomAdd("Double", result.Double));
                        ContactRandomizer.AddRange(RandomAdd("Triple", result.Triple));

                        HitType = ContactRandomizer[rnd.Next(ContactTotal)];

                        AbResultString = HitType;

                        resultString += "<br>" + HitType;

                        int resultTypeId = db.BattingResultTypes.Where(br => br.Title.Equals(sResult.Result)).FirstOrDefault().Id;
                        ab.resultId = db.BattingResults.Where(br => br.Title.Equals(HitType)).FirstOrDefault().Id;
                        ab.resultTypeId = resultTypeId;

                        switch (HitType)
                        {
                            case "Single":
                                Points += 8;
                                ab = MoveBaseRunners(ab, 1, sResult.ResultId, sResult.ToPosition);
                                ab.AtFirst = bA.playerId;
                                OnFirst = bA.playerId;
                                OnSecond = ab.AtSecond;
                                OnThird = ab.AtThird;
                                AtBatResult = AtBatResultNum.Single;
                                break;
                            case "Double":
                                Points += 11;
                                ab = MoveBaseRunners(ab, 2, sResult.ResultId, sResult.ToPosition);
                                ab.AtFirst = 0;
                                ab.AtSecond = bA.playerId;
                                OnSecond = bA.playerId;
                                OnThird = ab.AtThird;
                                AtBatResult = AtBatResultNum.Double;
                                break;
                            case "Triple":
                                Points += 15;
                                ab = MoveBaseRunners(ab, 3, sResult.ResultId, sResult.ToPosition);
                                ab.AtFirst = 0;
                                ab.AtSecond = 0;
                                ab.AtThird = bA.playerId;
                                OnThird = bA.playerId;
                                AtBatResult = AtBatResultNum.Triple;
                                break;
                        }

                        AtBatOver = true;
                    }
                }
            }
            else
            {
                if (!Swing && Strike)
                {
                    PitchResult = PitchResultNum.Strike;
                }
                else if (Swing && sResult.Result == "Miss")
                {
                    PitchResult = PitchResultNum.Miss;
                }
                else if (Swing && sResult.Result == "Foul")
                {
                    PitchResult = PitchResultNum.Foul;
                }
                else if (!Swing && !Strike)
                {
                    PitchResult = PitchResultNum.Ball;
                }

                int prTypeId = 0;
                if ((!Swing && Strike) || (Swing && (sResult.Result == "Foul" || sResult.Result == "Miss")))
                {
                    Strikes++;
                    prTypeId = 1;
                }
                else if (!Swing && !Strike)
                {
                    Balls++;
                    prTypeId = 2;
                }
                thrown.resultTypeId = prTypeId;

                if (Strikes == 3 && ((!Swing && Strike) || (Swing && sResult.Result == "Miss")))
                {
                    Points -= 3;
                    ab.resultId = 1;
                    ab.resultTypeId = 1;
                    ab.Innings.Outs++;
                    AtBatOver = true;
                    AtBatResult = AtBatResultNum.Strikeout;
                    AbResultString = "Strikeout";
                    resultString += "<br>Strikeout";
                }
                else if (Strikes == 3 && sResult.Result == "Foul")
                {
                    Strikes = 2;
                }

                if (Balls == 4)
                {
                    Points += 5;
                    //Balls++;
                    ab = MoveBaseRunners(ab, 1, 2, 0);
                    ab.AtFirst = bA.playerId;
                    //OnFirst = bA.playerId;
                    ab.resultId = 2;
                    ab.resultTypeId = 2;
                    AtBatOver = true;
                    AtBatResult = AtBatResultNum.Walk;
                    AbResultString = "Walk";
                    resultString += "<br>Walk";
                }
            }

            if (ab.Innings.TopOfInning)
            {
                ab.Game.ScoreAway += Score;
            }
            else
            {
                ab.Game.ScoreHome += Score;
            }

            db.PitchesThrowns.Add(thrown);

            if (ab.Batter.userId > 0)
            {
                if (ab.Batter.userId != User.plainUserId())
                {
                    Points = Points / 2;
                }
                ab.Batter.Points += Points;
                if (ab.Batter.Points < 0)
                {
                    ab.Batter.Points = 0;
                }
            }

            bool GameOver = false;
            if (ab.Innings.Number >= 9)
            {
                if (ab.Innings.TopOfInning && ab.Game.ScoreHome > ab.Game.ScoreAway && ab.Innings.Outs == 3)
                    GameOver = true;
                else if (!ab.Innings.TopOfInning && ab.Game.ScoreAway > ab.Game.ScoreHome && ab.Innings.Outs == 3)
                    GameOver = true;
                else if (!ab.Innings.TopOfInning && ab.Game.ScoreHome > ab.Game.ScoreAway)
                    GameOver = true;
            }

            if (GameOver)
                ab.Game.EndDate = DateTime.Now;

            ab.resultId = AtBatResult;
            thrown.resultTypeId = PitchResult;
            db.SaveChanges();

            PitchResult output = new PitchResult();
            //output.ResultType = sResult.Result;
            output.Result = resultString;
            output.InPlay = PitchResult == PitchResultNum.InPlay;
            output.Inning = ab.Innings.Number;
            output.Outs = ab.Innings.Outs;
            output.Balls = Balls;
            output.Strikes = Strikes;
            output.GameId = gameId;
            output.Over = AtBatOver;
            output.OnFirst = ab.AtFirst;
            output.OnSecond = ab.AtSecond;
            output.OnThird = ab.AtThird;
            output.Scored = Scored;
            output.X = pitchPosition.X;
            output.Y = pitchPosition.Y;
            output.Strike = Strike;
            output.Swing = Swing;
            output.GameOver = GameOver;
            output.IsUserPlayer = ab.Batter.userId == User.plainUserId();
            output.BatterName = ab.Batter.FirstName + ' ' + ab.Batter.LastName;
            output.AbResult = AbResultString;

            return output;
        }

        private PitchPosition GetPitchPosition(PlayerPitch pPitch, Player pitcher, decimal Mod)
        {
            PitchPosition endPosition = new PitchPosition();
            decimal BBRatio = pitcher.PlayerAttributes.First().BB9.valReversal() * pPitch.Pitch.BBRatio;
            decimal Control = pPitch.Control.valReversal() * Mod;

            int Xend = (15M * BBRatio).ToInt();
            int Yend = (20M * BBRatio).ToInt();

            //Random rnd = new Random();
            int Slot = rnd.Next(1, 9);

            switch (Slot)
            {
                case 1:
                    endPosition.X = 15 - Xend;
                    endPosition.Y = 100 + Yend;
                    break;
                case 2:
                    endPosition.X = 45;
                    endPosition.Y = 100 + Yend;
                    break;
                case 3:
                    endPosition.X = 75 + Xend;
                    endPosition.Y = 100 + Yend;
                    break;
                case 4:
                    endPosition.X = 15 - Xend;
                    endPosition.Y = 60;
                    break;
                case 5:
                    endPosition.X = 45;
                    endPosition.Y = 60;
                    break;
                case 6:
                    endPosition.X = 75 + Xend;
                    endPosition.Y = 60;
                    break;
                case 7:
                    endPosition.X = 15 - Xend;
                    endPosition.Y = 20 - Yend;
                    break;
                case 8:
                    endPosition.X = 45;
                    endPosition.Y = 20 + Yend;
                    break;
                case 9:
                    endPosition.X = 75 + Xend;
                    endPosition.Y = 20 - Yend;
                    break;
            }

            int PlacementCircle = (35M * Control).ToInt();

            int newX = rnd.Next(PlacementCircle * -1, PlacementCircle);
            int newY = rnd.Next(PlacementCircle * -1, PlacementCircle);

            int MaxMoveX = pPitch.Pitch.MovementX;
            int MaxMoveY = pPitch.Pitch.MovementY;
            if (pitcher.Throwing == "L")
            {
                MaxMoveX = MaxMoveX * -1;
                MaxMoveY = MaxMoveY * -1;
            }

            int myMaxX = (MaxMoveX.ToDecimal() * pPitch.Movement.valModifier() * pitcher.PlayerAttributes.First().K9.valModifier(-2)).ToInt();
            int myMaxY = (MaxMoveY.ToDecimal() * pPitch.Movement.valModifier() * pitcher.PlayerAttributes.First().K9.valModifier(-2)).ToInt();

            int myMinX = (myMaxX.ToDecimal() * .45M).ToInt();
            int myMinY = (myMaxY.ToDecimal() * .45M).ToInt();

            int movedX = 0;
            int movedY = 0;

            if (myMinX < myMaxX)
            {
                movedX = rnd.Next(myMinX, myMaxX);
            }
            else if (myMinX > myMaxX)
            {
                movedX = rnd.Next(myMaxX, myMinX);
            }
            else
            {
                movedX = 0;
            }

            if (myMinY < myMaxY)
            {
                movedY = rnd.Next(myMinY, myMaxY);
            }
            else if (myMinY > myMaxY)
            {
                movedY = rnd.Next(myMaxY, myMinY);
            }
            else
            {
                movedY = 0;
            }

            endPosition.X += newX;
            endPosition.Y += newY;
            endPosition.startX = endPosition.X - movedX;
            endPosition.startY = endPosition.Y - movedY;

            return endPosition;
        }

        private SwingResult BatterContact(PitchPosition position, PitchVelocity velocity, AtBat ab, PlayerPitch pPitch, decimal conMissMod, decimal powMod)
        {
            decimal ContactMiss = 0M;
            decimal bPower = 0M;

            PlayerAttribute bA = ab.Batter.PlayerAttributes.First();

            switch (ab.Pitcher.Throwing)
            {
                case "L":
                    ContactMiss = bA.ContactVsLeft.ToDecimal();
                    bPower = bA.PowerVsLeft.valModifier();
                    break;
                case "R":
                    ContactMiss = bA.ContactVsRight.ToDecimal();
                    bPower = bA.PowerVsRight.valModifier();
                    break;
            }

            ContactMiss = ContactMiss.valReversal() * conMissMod;

            int TotalMovementX = position.startX - position.X;
            if (TotalMovementX < 0) TotalMovementX = TotalMovementX * -1;

            int TotalMovementY = position.startY - position.Y;
            if (TotalMovementY < 0) TotalMovementY = TotalMovementY * -1;

            int TotalMovement = TotalMovementX + TotalMovementY;

            decimal mContactMiss = (1M + (TotalMovement / 100M));
            decimal vContactMiss = (1M + (((15 - (100 - velocity.Speed)) * 3).ToDecimal() / 100M));

            int FromX = position.startX - 35;
            int FromY = position.startY - 50;

            if (FromX < 0) FromX = FromX * -1;
            if (FromY < 0) FromY = FromY * -1;

            decimal pContactMiss = 0;

            int Furthest = FromX;
            if (FromY > FromX) Furthest = FromY;

            pContactMiss = 1M + (Furthest.ToDecimal() / 300M);

            int MissPixels = 10;

            MissPixels = (MissPixels.ToDecimal() * bA.PlateVision.valReversal() * ContactMiss * mContactMiss * vContactMiss * pContactMiss).ToInt();

            int s = MissPixels;

            //Random rnd = new Random();
            int XMiss = rnd.Next(MissPixels * -1, MissPixels);
            int YMiss = rnd.Next(MissPixels * -1, MissPixels);
            int ResultId = 0;

            string swingResult = "Miss";

            if (XMiss <= -25 || XMiss >= 25)
            {
                swingResult = "Miss";
            }
            else if (YMiss >= 8 && YMiss <= 13)
            {
                swingResult = "Foul";
                ResultId = SwingResultNum.Foul;
            }
            else if (YMiss >= 6 && YMiss <= 77)
            {
                swingResult = "Chopper";
                ResultId = SwingResultNum.Chopper;
            }
            else if (YMiss <= 5 && YMiss >= 2)
            {
                swingResult = "Ground";
                ResultId = SwingResultNum.Ground;
            }
            else if (YMiss <= 1 && YMiss >= -1)
            {
                swingResult = "Line";
                ResultId = SwingResultNum.Line;
            }
            else if (YMiss <= -2 && YMiss >= -5)
            {
                swingResult = "Fly";
                ResultId = SwingResultNum.Fly;
            }
            else if (YMiss <= -6 && YMiss >= -7)
            {
                swingResult = "Popup";
                ResultId = SwingResultNum.Popup;
            }
            else if (YMiss <= -8 && YMiss >= -13)
            {
                swingResult = "Foul";
                ResultId = SwingResultNum.Foul;
            }

            if (XMiss < 0)
            {
                XMiss = XMiss * -1;
            }

            int Power = ((100 - (XMiss * 4)).ToDecimal() * bPower * powMod).ToInt();

            SwingResult output = new SwingResult();
            output.Result = swingResult;
            output.Power = Power;
            output.ToPosition = 0;

            decimal timeToBatter = velocity.timeToBatter;
            int timeVar = (((1M - (timeToBatter)) - .1M) * .1M * 1000M / mContactMiss).ToInt();

            int swingTime = rnd.Next(timeVar * -1, timeVar);

            if (ab.Batter.Throwing == "L")
            {
                swingTime = swingTime * -1;
            }

            switch (swingResult)
            {
                case "Fly":
                    if (swingTime >= -25 && swingTime <= -9)
                    {
                        output.ToPosition = 7;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 7);
                        output.PlayDifficulty = swingTime.toSwingNum(-17M, 8M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * FlyD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= -8 && swingTime <= 8)
                    {
                        output.ToPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 8);
                        output.PlayDifficulty = swingTime.toSwingNum(0M, 8M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * FlyD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime <= 25 && swingTime >= 9)
                    {
                        output.ToPosition = 9;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 9);
                        output.PlayDifficulty = swingTime.toSwingNum(18M, 8) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * FlyD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    break;
                case "Popup":
                    if (swingTime >= -25 && swingTime <= -13)
                    {
                        output.ToPosition = 5;
                        output.SecondaryPosition = 7;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 5);
                        output.PlayDifficulty = swingTime.toSwingNum(-19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * PopD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= -12 && swingTime <= -1)
                    {
                        output.ToPosition = 6;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 6);
                        output.PlayDifficulty = swingTime.toSwingNum(-6.5M, 5.5M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * PopD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 0 && swingTime <= 12)
                    {
                        output.ToPosition = 4;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 4);
                        output.PlayDifficulty = swingTime.toSwingNum(6M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * PopD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 13 && swingTime <= 25)
                    {
                        output.ToPosition = 4;
                        output.SecondaryPosition = 9;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 4);
                        output.PlayDifficulty = swingTime.toSwingNum(19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * PopD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    break;
                case "Line":
                    if (swingTime >= -25 && swingTime <= -13)
                    {
                        output.ToPosition = 5;
                        output.SecondaryPosition = 7;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 5);
                        output.PlayDifficulty = swingTime.toSwingNum(-19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * LineD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= -12 && swingTime <= -1)
                    {
                        output.ToPosition = 6;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 6);
                        output.PlayDifficulty = swingTime.toSwingNum(-6.5M, 5.5M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * LineD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 0 && swingTime <= 12)
                    {
                        output.ToPosition = 4;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 4);
                        output.PlayDifficulty = swingTime.toSwingNum(6M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * LineD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 13 && swingTime <= 25)
                    {
                        output.ToPosition = 3;
                        output.SecondaryPosition = 9;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 3);
                        output.PlayDifficulty = swingTime.toSwingNum(19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * LineD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    break;
                case "Ground":
                    if (swingTime >= -25 && swingTime <= -14)
                    {
                        output.ToPosition = 5;
                        output.SecondaryPosition = 7;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 5);
                        output.PlayDifficulty = swingTime.toSwingNum(-19.5M, 5.5M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * GroundD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= -13 && swingTime <= -1)
                    {
                        output.ToPosition = 6;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 6);
                        output.PlayDifficulty = swingTime.toSwingNum(-7M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * GroundD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime == 0)
                    {
                        output.ToPosition = 1;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 1);
                        output.PlayDifficulty = .2M * Power.valModifier() * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-2) * dA.Fielding.valModifier(-2) * PGroundD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 1 && swingTime <= 13)
                    {
                        output.ToPosition = 4;
                        output.SecondaryPosition = 8;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 4);
                        output.PlayDifficulty = swingTime.toSwingNum(7M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * GroundD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 13 && swingTime <= 25)
                    {
                        output.ToPosition = 3;
                        output.SecondaryPosition = 9;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 3);
                        output.PlayDifficulty = swingTime.toSwingNum(19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * GroundD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    break;
                case "Chopper":
                    if (swingTime >= -25 && swingTime <= -13)
                    {
                        output.ToPosition = 5;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 5);
                        output.PlayDifficulty = swingTime.toSwingNum(-19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * ChopperD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= -12 && swingTime <= -1)
                    {
                        output.ToPosition = 6;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 6);
                        output.PlayDifficulty = swingTime.toSwingNum(-6.5M, 5.5M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * ChopperD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 0 && swingTime <= 12)
                    {
                        output.ToPosition = 4;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 4);
                        output.PlayDifficulty = swingTime.toSwingNum(6M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * ChopperD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    if (swingTime >= 13 && swingTime <= 25)
                    {
                        output.ToPosition = 4;
                        PlayerAttribute dA = Defender(ab.Game, ab.Game.homeTeamId == ab.Batter.teamId, 4);
                        output.PlayDifficulty = swingTime.toSwingNum(19M, 6M) * Power.valModifier(-3) * dA.Speed.valReversal(-2) * dA.Reaction.valReversal(-3) * dA.Fielding.valModifier(-3) * ChopperD;
                        if (output.PlayDifficulty < 0)
                        {
                            output.PlayDifficulty = output.PlayDifficulty * -1;
                        }
                    }
                    break;
            }

            if (output.ToPosition == 0 && output.Result != "Miss")
            {
                output.Result = "Foul";
            }
            else
            {
                output.ResultId = ResultId;
            }

            return output;
        }

        private PlayerAttribute Defender(Game game, bool home, int positionId)
        {
            if (home)
            {
                return game.Lineups.Where(l => l.teamId.Equals(game.homeTeamId) && l.positionId.Equals(positionId)).First().Player.PlayerAttributes.First();
            }
            else
            {
                return game.Lineups.Where(l => l.teamId.Equals(game.awayTeamId) && l.positionId.Equals(positionId)).First().Player.PlayerAttributes.First();
            }
        }

        private bool SwingBatter(PitchPosition position, Player batter, AtBat ab, decimal Mod)
        {
            decimal CountRatio = 0;

            int Balls = ab.PitchesThrowns.Where(p => p.resultTypeId.Equals(2)).Count();
            int Strikes = ab.PitchesThrowns.Where(p => p.resultTypeId.Equals(1) || p.resultTypeId.Equals(3) || p.resultTypeId.Equals(4)).Count();
            if (Strikes > 2)
                Strikes = 2;

            switch (Balls)
            {
                case 0:
                    switch (Strikes)
                    {
                        case 0:
                            CountRatio = .35M;
                            break;
                        case 1:
                            CountRatio = .5M;
                            break;
                        case 2:
                            CountRatio = .7M;
                            break;
                    }
                    break;
                case 1:
                    switch (Strikes)
                    {
                        case 0:
                            CountRatio = .3M;
                            break;
                        case 1:
                            CountRatio = .5M;
                            break;
                        case 2:
                            CountRatio = .67M;
                            break;
                    }
                    break;
                case 2:
                    switch (Strikes)
                    {
                        case 0:
                            CountRatio = .25M;
                            break;
                        case 1:
                            CountRatio = .48M;
                            break;
                        case 2:
                            CountRatio = .65M;
                            break;
                    }
                    break;
                case 3:
                    switch (Strikes)
                    {
                        case 0:
                            CountRatio = .2M;
                            break;
                        case 1:
                            CountRatio = .35M;
                            break;
                        case 2:
                            CountRatio = .63M;
                            break;
                    }
                    break;
            }

            int FromX = position.startX - 35;
            int FromY = position.startY - 50;

            if (FromX < 0) FromX = FromX * -1;
            if (FromY < 0) FromY = FromY * -1;

            decimal posRatio = 0;

            if (FromX > 10 || FromY > 10)
            {
                posRatio = batter.PlayerAttributes.FirstOrDefault().PlateDiscipline.valReversal();
            }
            else
            {
                posRatio = batter.PlayerAttributes.FirstOrDefault().PlateVision.valModifier();
            }

            int Furthest = FromX;
            if (FromY > FromX) Furthest = FromY;

            posRatio = posRatio * ((100M - (Furthest.ToDecimal() / 3M)) / 100M);

            int DoesSwing = (CountRatio * posRatio * Mod * 100M).ToInt();
            List<bool> sList = new List<bool>();
            for (int i = 1; i <= DoesSwing; i++)
            {
                sList.Add(true);
            }

            for (int i = 1; i <= 100 - DoesSwing; i++)
            {
                sList.Add(false);
            }
            //Random rnd = new Random();

            return sList[rnd.Next(100)];
        }

        private PitchVelocity GetPitchVelocity(PlayerPitch pPitch, decimal Mod)
        {
            decimal SpeedDiff = (pPitch.Pitch.MaxSpeed - pPitch.Pitch.MinSpeed).ToDecimal();
            int pPitchSpeed = (((pPitch.Speed.ToDecimal() / 100M) * SpeedDiff) + pPitch.Pitch.MinSpeed.ToDecimal() + Mod).ToInt();
            //Random rnd = new Random();

            int outSpeed = rnd.Next(pPitchSpeed - 6, pPitchSpeed);
            PitchVelocity vel = new PitchVelocity();
            vel.Speed = outSpeed;
            vel.timeToBatter = 41.25M / outSpeed.ToDecimal();
            return vel;
        }

        private class PitchPosition
        {
            public int startX { get; set; }
            public int startY { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        private class PitchVelocity
        {
            public decimal timeToBatter { get; set; }
            public int Speed { get; set; }
        }

        private class SwingResult
        {
            public string Result { get; set; }
            public int Power { get; set; }
            public int ToPosition { get; set; }
            public decimal PlayDifficulty { get; set; }
            public int ResultId { get; set; }
            public int SecondaryPosition { get; set; }
        }

        private AtBat ScoreRun(AtBat ab, Player runner)
        {
            BatBreakerContext db = new BatBreakerContext();
            RunsScored run = db.RunsScoreds.Create();
            run.gameId = ab.gameId;
            run.inningId = ab.inningId;
            run.abId = ab.Id;
            run.pitcherId = ab.pitcherId;
            run.runnerId = runner.Id;
            run.CreateDate = DateTime.Now;
            Points += 15;

            Scored += "<br>" + runner.LastName + " scores";

            db.RunsScoreds.Add(run);
            db.SaveChanges();

            int UserId = User.plainUserId();

            if (run.Runner.userId > 0)
            {
                if (run.Runner.userId == UserId)
                {
                    Points += 10;
                }
                else
                {
                    run.Runner.Points += 5;
                }
            }
            db.SaveChanges();

            Score++;

            return ab;
        }

        private AtBat MoveBaseRunners(AtBat ab, int Bases, int ResultTypeId, int PositionId)
        {
            //Random rnd = new Random();
            PlayerAttribute Defender = null;

            if (PositionId > 0)
            {
                Defender = ab.Game.Lineups.Where(l => l.teamId.Equals(ab.Pitcher.teamId) && l.positionId.Equals(PositionId)).First().Player.PlayerAttributes.FirstOrDefault();
            }

            BatBreakerContext db = new BatBreakerContext();
            BattingResultType type = db.BattingResultTypes.Where(b => b.Id.Equals(ResultTypeId)).FirstOrDefault();
            
            if (ResultTypeId == 2)
            {
                if (ab.AtFirst > 0 && ab.AtSecond > 0 && ab.AtThird > 0)
                {
                    ScoreRun(ab, ab.PlayerOnThird);

                    ab.AtThird = 0;
                }

                if (ab.AtSecond > 0 && ab.AtFirst > 0)
                {
                    ab.AtThird = ab.AtSecond;
                }

                if (ab.AtFirst > 0)
                {
                    ab.AtSecond = ab.AtFirst;
                }

                return ab;
            }
            else if (Bases == 4)
            {
                if (ab.AtThird > 0)
                {
                    ScoreRun(ab, ab.PlayerOnThird);

                    ab.AtThird = 0;

                }

                if (ab.AtSecond > 0)
                {
                    ScoreRun(ab, ab.PlayerOnSecond);

                    ab.AtSecond = 0;
                }

                if (ab.AtFirst > 0)
                {
                    ScoreRun(ab, ab.PlayerOnFirst);

                    ab.AtFirst = 0;

                }
            }
            else
            {
                decimal RThirdBases = Bases;
                decimal RSecondBases = Bases;
                decimal RFirstBases = Bases;
                if (ab.AtThird > 0)
                {
                    PlayerAttribute pta = ab.PlayerOnThird.PlayerAttributes.FirstOrDefault();
                    decimal ptM = pta.Speed.valModifier() * pta.BRAgressiveness.valModifier() * pta.BRAbility.valModifier() * Defender.ArmAccuracy.valReversal() * Defender.ArmPower.valReversal();
                    if (Bases > 0)
                    {
                        if (ResultTypeId == 4)
                        {
                            ptM = ptM * 1.2M;
                        }
                        if (ResultTypeId == SwingResultNum.Line && PositionId < 7)
                        {
                            ptM = ptM * .5M;
                        }
                        RThirdBases += type.RunnerThirdExtraBases * ptM;
                    }
                    else
                    {
                        RThirdBases += type.RunnerThirdBasesOnOut * ptM;
                        if (RThirdBases > 1)
                        {
                            RThirdBases = 1;
                        }
                    }
                }

                if (ab.AtSecond > 0)
                {
                    PlayerAttribute psa = ab.PlayerOnSecond.PlayerAttributes.FirstOrDefault();
                    decimal psM = psa.Speed.valModifier() * psa.BRAgressiveness.valModifier() * psa.BRAbility.valModifier() * Defender.ArmAccuracy.valReversal() * Defender.ArmPower.valReversal();
                    if (Bases > 0)
                    {
                        if (ResultTypeId == 4 && (PositionId == 4 || PositionId == 3))
                        {
                            psM = psM * 1.5M;
                        }
                        RSecondBases += type.RunnerSecondExtraBases * psM;
                    }
                    else
                    {
                        RSecondBases += type.RunnerSecondBasesOnOut * psM;
                        if (RSecondBases > 1)
                        {
                            RSecondBases = 1;
                        }
                    }
                }

                if (ab.AtFirst > 0)
                {
                    PlayerAttribute pfa = ab.PlayerOnFirst.PlayerAttributes.FirstOrDefault();
                    decimal pfM = pfa.Speed.valModifier() * pfa.BRAgressiveness.valModifier() * pfa.BRAbility.valModifier() * Defender.ArmAccuracy.valReversal() * Defender.ArmPower.valReversal();
                    if (Bases > 0)
                    {

                        RFirstBases += type.RunnerFirstExtraBases * pfM;
                    }
                    else
                    {
                        RFirstBases += type.RunnerFirstBasesOnOut * pfM;
                        if (RFirstBases > 1)
                        {
                            RFirstBases = 1;
                        }
                    }
                }

                int ThirdBases = Math.Floor(double.Parse(RThirdBases.ToString())).ToInt();
                int SecondBases = Math.Floor(double.Parse(RSecondBases.ToString())).ToInt();
                int FirstBases = Math.Floor(double.Parse(RFirstBases.ToString())).ToInt();

                if (ab.AtThird > 0 && ThirdBases > 0)
                {
                    ScoreRun(ab, ab.PlayerOnThird);

                    ab.AtThird = 0;
                }

                if (ab.AtSecond > 0 && ab.AtThird == 0)
                {
                    if (SecondBases == 1)
                    {
                        ab.AtThird = ab.AtSecond;
                        ab.AtSecond = 0;
                    }
                    else if (SecondBases > 1)
                    {
                        ScoreRun(ab, ab.PlayerOnSecond);
                        ab.AtSecond = 0;
                    }
                }

                if (ab.AtFirst > 0 && ab.AtSecond == 0)
                {
                    if (FirstBases == 1 || (FirstBases > 1 && ab.AtThird > 0))
                    {
                        ab.AtSecond = ab.AtFirst;
                        ab.AtFirst = 0;
                    }
                    else if (FirstBases == 2 && ab.AtThird == 0)
                    {
                        ab.AtThird = ab.AtFirst;
                        ab.AtFirst = 0;
                    }
                    else if (FirstBases > 2 && ab.AtThird == 0)
                    {
                        ScoreRun(ab, ab.PlayerOnFirst);
                        ab.AtFirst = 0;
                    }
                }
            }

            return ab;
        }

        private List<string> RandomAdd(string type, int count)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < count; i++)
            {
                output.Add(type);
            }

            return output;
        }

    }
}
