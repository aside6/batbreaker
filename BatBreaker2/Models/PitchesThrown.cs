namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PitchesThrown")]
    public partial class PitchesThrown
    {
        public int pitcherId { get; set; }

        public int batterId { get; set; }

        public int abId { get; set; }

        public int pitchId { get; set; }

        public int Id { get; set; }

        public int Speed { get; set; }

        public int resultTypeId { get; set; }

        public int startX { get; set; }

        public int startY { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Power { get; set; }

        public int Bases { get; set; }

        public int Runs { get; set; }

        public int EarnedRuns { get; set; }

        public decimal PlayDifficulty { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual AtBat AtBat { get; set; }

        public virtual Pitch Pitch { get; set; }

        public virtual Player Player { get; set; }

        public virtual Player Player1 { get; set; }

        public virtual PitchResultType PitchResultType { get; set; }
    }
}
