namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlayerPitch
    {
        public int playerId { get; set; }

        public int pitchId { get; set; }

        public int Id { get; set; }

        public int Speed { get; set; }

        public int Control { get; set; }

        public int Movement { get; set; }

        public virtual Pitch Pitch { get; set; }

        public virtual Player Player { get; set; }
    }
}
