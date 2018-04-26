namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Game
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Game()
        {
            AtBats = new HashSet<AtBat>();
            Innings = new HashSet<Innings>();
            Lineups = new HashSet<Lineup>();
            RunsScoreds = new HashSet<RunsScored>();
        }

        public int userId { get; set; }

        public int scheduleId { get; set; }

        public int homeTeamId { get; set; }

        public int awayTeamId { get; set; }

        public int Id { get; set; }

        public Guid guId { get; set; }

        public int ScoreHome { get; set; }

        public int ScoreAway { get; set; }

        public int Outs { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AtBat> AtBats { get; set; }

        public virtual Team HomeTeam { get; set; }

        public virtual Team AwayTeam { get; set; }

        public virtual Schedule Schedule { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Innings> Innings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lineup> Lineups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RunsScored> RunsScoreds { get; set; }
    }
}
