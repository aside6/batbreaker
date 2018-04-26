namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class City
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string CityName { get; set; }
    }
}
