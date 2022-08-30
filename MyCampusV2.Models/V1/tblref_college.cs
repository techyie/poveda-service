using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_college")]
    public class tblref_college : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("College_ID")]
        public int College_ID { get; set; }

        [Required]
        [StringLength(150)]
        [Column("College_Name")]
        public string College_Name { get; set; }

        [StringLength(10)]
        [Column("College_Status")]
        public string College_Status { get; set; }

        [Required]
        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [Required]
        [Column("Level_ID")]
        public int Level_ID { get; set; }

        //[ForeignKey("Level_ID")]
        //public virtual tbl_educ_level tbl_educ_level { get; set; }

        //public virtual ICollection<tblref_course> tblref_course { get; set; }
    }
}
