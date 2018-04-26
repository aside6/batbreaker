namespace BatBreaker2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Pitch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pitch()
        {
            PitchesThrowns = new HashSet<PitchesThrown>();
            PlayerPitches = new HashSet<PlayerPitch>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public int MinSpeed { get; set; }

        public int MaxSpeed { get; set; }

        public int MovementX { get; set; }

        public int MovementY { get; set; }

        public decimal KRatio { get; set; }

        public decimal HRatio { get; set; }

        public decimal HRRatio { get; set; }

        public decimal BBRatio { get; set; }

        public decimal PopupRatio { get; set; }

        public decimal GroundRatio { get; set; }

        public decimal FlyRatio { get; set; }

        public decimal ChopperRatio { get; set; }

        public decimal LineRadio { get; set; }

        public decimal FoulRatio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PitchesThrown> PitchesThrowns { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerPitch> PlayerPitches { get; set; }
    }
}
