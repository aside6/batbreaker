namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FirstName
    {
        public int Id { get; set; }

        [Column("FirstName")]
        [StringLength(255)]
        public string FirstName1 { get; set; }
    }
}
