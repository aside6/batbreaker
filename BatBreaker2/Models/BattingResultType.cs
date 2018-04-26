namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BattingResultType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BattingResultType()
        {
            AtBats = new HashSet<AtBat>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public decimal OutRatio { get; set; }

        public decimal SingleRatio { get; set; }

        public decimal DoubleRatio { get; set; }

        public decimal TripleRatio { get; set; }

        public decimal HRRatio { get; set; }

        public decimal DPRatio { get; set; }

        public decimal TPRatio { get; set; }

        public decimal RunnerThirdScoreRatio { get; set; }

        public decimal RunnerSecondScoreRatio { get; set; }

        public decimal RunnerSecondToThirdRatio { get; set; }

        public decimal RunnerFirstScoreRatio { get; set; }

        public decimal RunnerFirstToThirdRatio { get; set; }

        public decimal RunnerFirstToSecondRatio { get; set; }

        public decimal OutOfPlayRatio { get; set; }

        public decimal RunnerFirstExtraBases { get; set; }

        public decimal RunnerSecondExtraBases { get; set; }

        public decimal RunnerThirdExtraBases { get; set; }

        public decimal RunnerFirstBasesOnOut { get; set; }

        public decimal RunnerSecondBasesOnOut { get; set; }

        public decimal RunnerThirdBasesOnOut { get; set; }

        public decimal Bases { get; set; }

        public decimal ExtraBasesOnError { get; set; }

        public decimal ErrorRatio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AtBat> AtBats { get; set; }
    }
}
