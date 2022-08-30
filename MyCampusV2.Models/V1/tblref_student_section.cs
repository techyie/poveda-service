using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models
{
    //[Table("tblref_student_section")]
    public class tblref_student_section : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("StudSec_ID")]
        public int StudSec_ID { get; set; }

        [Required]
        [Column("Description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Column("YearSec_ID")]
        public int YearSec_ID { get; set; }

        [Required]
        [Column("Start_Time")]
        public DateTime Start_Time { get; set; }

        [Required]
        [Column("End_Time")]
        public DateTime End_Time { get; set; }

        [Required]
        [Column("Half_Day")]
        public DateTime Half_Day { get; set; }

        [Required]
        [Column("Grace_Period")]
        [StringLength(45)]
        public string Grace_Period { get; set; }

        //[ForeignKey("YearSec_ID")]
        //public virtual tblref_year_section tblref_year_section { get; set; }
    }
}
