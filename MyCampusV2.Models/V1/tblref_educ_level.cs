using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_educ_level")]
    public class tblref_educ_level : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Level_ID")]
        public int Level_ID { get; set; }

        [Required]
        [Column("Level_Name")]
        [StringLength(100)]
        public string Level_Name { get; set; }

        [Column("Level_Status")]
        [StringLength(10)]
        public string Level_Status { get; set; }

        [Required]
        [Column("IsCollege")]
        public bool IsCollege { get; set; }

        [Required]
        [Column("hasCourse")]
        public bool hasCourse { get; set; }

        [Required]
        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tblref_campus { get; set; }

        //public virtual ICollection<tblref_year_level> tblref_year_level { get; set; }
        //public virtual ICollection<tblref_college> tblref_college { get; set; }
    }
}
