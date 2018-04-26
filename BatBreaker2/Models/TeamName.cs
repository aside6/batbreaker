namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TeamName
    {
        public int Id { get; set; }

        [Column("TeamName")]
        [StringLength(255)]
        public string TeamName1 { get; set; }
    }
}
