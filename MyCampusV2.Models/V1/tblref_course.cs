using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_course")]
    public class tblref_course : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Course_ID")]
        public int Course_ID { get; set; }

        [Required]
        [StringLength(150)]
        [Column("Course_Name")]
        public string Course_Name { get; set; }

        [StringLength(10)]
        [Column("Course_Status")]
        public string Course_Status { get; set; }

        [Required]
        [Column("College_ID")]
        public int College_ID { get; set; }

        //[ForeignKey("College_ID")]
        //public virtual tblref_college tblref_college { get; set; }
    }
}
