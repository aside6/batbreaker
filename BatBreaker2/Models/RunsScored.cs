namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RunsScored")]
    public partial class RunsScored
    {
        public int gameId { get; set; }

        public int inningId { get; set; }

        public int abId { get; set; }

        public int runnerId { get; set; }

        public int pitcherId { get; set; }

        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual AtBat AtBat { get; set; }

        public virtual Game Game { get; set; }

        public virtual Innings Innings { get; set; }

        public virtual Player Runner { get; set; }

        public virtual Player Player1 { get; set; }
    }
}
