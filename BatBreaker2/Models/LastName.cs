namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LastName
    {
        public int Id { get; set; }

        [Column("LastName")]
        [StringLength(255)]
        public string LastName1 { get; set; }
    }
}
