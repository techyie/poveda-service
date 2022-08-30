using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_educ_level")]
    public class tbl_educ_level : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Educ_Level_Name")]
        [StringLength(125)]
        public string Educ_Level_Name { get; set; }

        [Column("Educ_Level_Desc")]
        [StringLength(125)]
        public string Educ_Level_Desc { get; set; }

        [Column("IsCollege")]
        public Boolean IsCollege { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tblref_campus { get; set; }

        //public virtual ICollection<tblref_year_level> tblref_year_level { get; set; }
        //public virtual ICollection<tbl_college> tbl_college { get; set; }
    }
}
