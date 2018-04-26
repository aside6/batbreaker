namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AtBat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AtBat()
        {
            BaseRunnings = new HashSet<BaseRunning>();
            DefensivePlays = new HashSet<DefensivePlay>();
            PitchesThrowns = new HashSet<PitchesThrown>();
            RunsScoreds = new HashSet<RunsScored>();
        }

        public int userId { get; set; }

        public int gameId { get; set; }

        public int inningId { get; set; }

        public int pitcherId { get; set; }

        public int batterId { get; set; }

        public int Id { get; set; }

        public int Outs { get; set; }

        public int AtFirst { get; set; }

        public int AtSecond { get; set; }

        public int AtThird { get; set; }

        public int resultTypeId { get; set; }

        public int resultId { get; set; }

        public int Bases { get; set; }

        public int Runs { get; set; }

        public int EarnedRuns { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual Player Pitcher { get; set; }

        public virtual Player Batter { get; set; }

        public virtual Player PlayerOnFirst { get; set; }

        public virtual Player PlayerOnSecond { get; set; }

        public virtual BattingResult BattingResult { get; set; }

        public virtual BattingResultType BattingResultType { get; set; }

        public virtual Game Game { get; set; }

        public virtual Innings Innings { get; set; }

        public virtual Player PlayerOnThird { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaseRunning> BaseRunnings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DefensivePlay> DefensivePlays { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PitchesThrown> PitchesThrowns { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RunsScored> RunsScoreds { get; set; }
    }
}
