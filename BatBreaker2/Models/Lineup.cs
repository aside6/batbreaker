namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Lineup
    {
        public int gameId { get; set; }

        public int teamId { get; set; }

        public int Id { get; set; }

        public int playerId { get; set; }

        public int positionId { get; set; }

        public int BattingOrder { get; set; }

        public bool Deactivated { get; set; }

        public virtual Game Game { get; set; }

        public virtual Player Player { get; set; }

        public virtual Position Position { get; set; }

        public virtual Team Team { get; set; }
    }
}
