using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_emergency_logout")]
    public class emergencyLogoutEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Emergency_logout_ID")]
        public int Emergency_logout_ID { get; set; }

        [Required]
        [Column("Remarks")]
        [StringLength(250)]
        public string Remarks { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Column("IsCancelled")]
        public bool IsCancelled { get; set; }

        [Column("EffectivityDate")]
        public DateTime EffectivityDate { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }

        [ForeignKey("Person_ID")]
        public virtual personEntity PersonEntity { get; set; }
    }
}