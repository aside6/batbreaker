namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DefensivePlay
    {
        public int abId { get; set; }

        public int playerId { get; set; }

        public int Id { get; set; }

        public int Result { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual AtBat AtBat { get; set; }

        public virtual Player Player { get; set; }
    }
}
