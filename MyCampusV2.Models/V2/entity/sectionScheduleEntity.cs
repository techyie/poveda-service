using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_section_schedule")]
    public class sectionScheduleEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }
        
        [Required]
        [Column("Date")]
        public DateTime Schedule_Date { get; set; }

        [NotMapped]
        public DateTime Schedule_Start_Date { get; set; }

        [NotMapped]
        public DateTime Schedule_End_Date { get; set; }

        [Column("Start_Time")]
        public TimeSpan Start_Time { get; set; }

        [Column("End_Time")]
        public TimeSpan End_Time { get; set; }

        [Column("Half_Day")]
        public TimeSpan Half_Day { get; set; }

        [Column("Grace_Period")]
        [StringLength(45)]
        public string Grace_Period { get; set; }
        
        [Column("isExcused")]
        public bool IsExcused { get; set; }

        [Column("Remarks")]
        [StringLength(45)]
        public string Remarks { get; set; }

        [Column("StudSec_ID")]
        public int StudSec_ID { get; set; }

        [ForeignKey("StudSec_ID")]
        public virtual studentSectionEntity StudentSectionEntity { get; set; }
    }
}
