using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_college")]
    public class tbl_college : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(125)]
        [Column("College_Name")]
        public string College_Name { get; set; }

        [StringLength(125)]
        [Column("College_Desc")]
        public string College_Desc { get; set; }

        [Column("Educ_Level_ID")]
        public int Educ_Level_ID { get; set; }

        //[ForeignKey("Educ_Level_ID")]
        //public virtual tbl_educ_level tbl_educ_level { get; set; }

        //public virtual ICollection<tbl_college_course> tbl_college_course { get; set; }
    }
}
