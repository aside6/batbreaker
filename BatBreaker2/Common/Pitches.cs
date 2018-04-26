using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatBreaker2.Common
{
    public static class PitchResultNum
    {
        public const int Strike = 1;
        public const int Ball = 2;
        public const int Miss = 3;
        public const int Foul = 4;
        public const int InPlay = 5;
    }

    public static class AtBatResultNum
    {
        public const int Strikeout = 1;
        public const int Walk = 2;
        public const int Out = 3;
        public const int Single = 4;
        public const int Double = 5;
        public const int Triple = 6;
        public const int HomeRun = 7;
    }

    public static class SwingResultNum
    {
        public const int Strikeout = 1;
        public const int Walk = 2;
        public const int Popup = 3;
        public const int Ground = 4;
        public const int Fly = 5;
        public const int Chopper = 6;
        public const int Line = 8;
        public const int Foul = 9;
    }

    public class Pitches
    {
        public int Strike { get; set; }
        public int Ball { get; set; }
        public int Popup { get; set; }
        public int Ground { get; set; }
        public int Fly { get; set; }
        public int Chopper { get; set; }
        public int Line { get; set; }
        public int Foul { get; set; }           
    }

    public class AbResult
    {
        public int Walk { get; set; }
        public int Strikeout { get; set; }
        public int Single { get; set; }
        public int Double { get; set; }
        public int Triple { get; set; }
        public int HomeRun { get; set; }
        public int Out { get; set; }
        public int OutOfPlay { get; set; }
    }

    public class PitchResult
    {
        public string ResultType { get; set; }
        public string Result { get; set; }
        public int Strikes { get; set; }
        public int Balls { get; set; }
        public int Outs { get; set; }
        public int Inning { get; set; }
        public bool Over { get; set; }
        public Guid GameId { get; set; }
        public int OnFirst { get; set; }
        public int OnSecond { get; set; }
        public int OnThird { get; set; }
        public string Scored { get; set; }
        public bool GameOver { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Strike { get; set; }
        public bool Swing { get; set; }
        public bool IsUserPlayer { get; set; }
        public bool InPlay { get; set; }
        public string BatterName { get; set; }
        public string AbResult { get; set; }
        public string BatterGameStats { get; set; }
    }
}