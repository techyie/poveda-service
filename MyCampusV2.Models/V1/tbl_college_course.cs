using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_college_course")]
    public class tbl_college_course : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        //[Required]
        [StringLength(30)]
        [Column("Course_Code")]
        public string Course_Code { get; set; }

        [Required]
        [StringLength(125)]
        [Column("Course_Name")]
        public string Course_Name { get; set; }

        [StringLength(125)]
        [Column("Course_Desc")]
        public string Course_Desc { get; set; }

        [Column("College_ID")]
        public int College_ID { get; set; }

        //[ForeignKey("College_ID")]
        //public virtual tbl_college tbl_college { get; set; }
    }
}
