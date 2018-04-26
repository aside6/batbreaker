using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatBreaker2.Models
{
    public class StatModels
    {
        public class PlayerAttribute
        {
            public string Type { get; set; }
            public int Amount { get; set; }
        }
        public class BatterStats
        {
            public Player player { get; set; }
            public decimal Avg { get; set; }
            public decimal SLG { get; set; }
            public decimal OBP { get; set; }
            public decimal OPS { get; set; }
            public int ABs { get; set; }
            public int Hits { get; set; }
            public int Singles { get; set; }
            public int Doubles { get; set; }
            public int Triples { get; set; }
            public int HR { get; set; }
            public int K { get; set; }
            public int BB { get; set; }
            public int TB { get; set; }
            public int R { get; set; }
            public int RBI { get; set; }
        }

        public class PitcherStats
        {
            public decimal ERA { get; set; }
            public decimal AvgAgainst { get; set; }
            public int HAgainst { get; set; }
            public int HRAgainst { get; set; }
            public int K { get; set; }
            public int BB { get; set; }
            public decimal IP { get; set; }
            public BatterStats BattingStats { get; set; }

            public PitcherStats()
            {
                BattingStats = new BatterStats();
            }
        }

        public class Standings
        {
            public int Division { get; set; }
            public Team team { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public decimal WinPct { get; set; }
        }
    }
}