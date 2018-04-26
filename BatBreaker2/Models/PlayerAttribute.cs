namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlayerAttribute
    {
        public int playerId { get; set; }

        public int Id { get; set; }

        public int Energy { get; set; }

        public int Confidence { get; set; }

        public int Stamina { get; set; }

        public int Durability { get; set; }

        public int H9 { get; set; }

        public int HR9 { get; set; }

        public int K9 { get; set; }

        public int BB9 { get; set; }

        public int PitchingClutch { get; set; }

        public int ContactVsLeft { get; set; }

        public int ContactVsRight { get; set; }

        public int PowerVsLeft { get; set; }

        public int PowerVsRight { get; set; }

        public int PlateDiscipline { get; set; }

        public int PlateVision { get; set; }

        public int BattingClutch { get; set; }

        public int Fielding { get; set; }

        public int Reaction { get; set; }

        public int Speed { get; set; }

        public int BRAbility { get; set; }

        public int BRAgressiveness { get; set; }

        public int ArmPower { get; set; }

        public int ArmAccuracy { get; set; }

        public int Blocking { get; set; }

        public int GameCalling { get; set; }

        public virtual Player Player { get; set; }
    }
}
