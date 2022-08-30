using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_datasync_emergency")]
    public class datasyncEmergencyEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("StudentSerial")]
        [StringLength(50)]
        public string StudentSerial { get; set; }

        [Column("EffectivityDate")]
        public DateTime EffectivityDate { get; set; }

        [Column("DS_Action")]
        [StringLength(1)]
        public string DS_Action { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }
    }
}