using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_student_section")]
    public class studentSectionEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("StudSec_ID")]
        public int StudSec_ID { get; set; }

        [Required]
        [Column("Description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Column("Start_Time")]
        public TimeSpan Start_Time { get; set; }

        [Column("End_Time")]
        public TimeSpan End_Time { get; set; }

        [Column("Half_Day")]
        public TimeSpan Half_Day { get; set; }

        [Column("Grace_Period")]
        [StringLength(45)]
        public string Grace_Period { get; set; }

        [Column("Password")]
        [StringLength(300)]
        public string Password { get; set; }

        /*
         * Student Section doesn't have status, so I just put a property only with no annotation.
         * Just turn on the annotation if you want to use this
         */
        //[Column("YearSec_Status")]
        //[StringLength(10)]
        //public string StudSec_Status { get; set; }

        [Column("YearSec_ID")]
        public int YearSec_ID { get; set; }

        //[ForeignKey("YearSec_ID")]
        //public virtual yearSectionEntity YearSectionEntity { get; set; }

        [ForeignKey("YearSec_ID")]
        public yearSectionEntity YearSectionEntity { get; set; }

        //public virtual ICollection<yearSectionEntity> YearSectionList { get; set; }
    }
}