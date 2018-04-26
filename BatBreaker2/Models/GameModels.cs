using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatBreaker2.Models
{
    public class GameModels
    {
        public class Lineups
        {
            public List<TeamLineup> Home { get; set; }
            public List<TeamLineup> Away { get; set; }

            public Lineups()
            {
                Home = new List<TeamLineup>();
                Away = new List<TeamLineup>();
            }
        }

        public class TeamLineup
        {
            public int playerId { get; set; }
            public string PlayerName { get; set; }
            public string Position { get; set; }
            public int BattingOrder { get; set; }
        }
    }
}