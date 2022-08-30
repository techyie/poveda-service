using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_college_course")]
    public class tbl_college_course_section : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(125)]
        [Column("Course_Section_Name")]
        public string Course_Section_Name { get; set; }

        [StringLength(125)]
        [Column("Course_Section_Desc")]
        public string Course_Section_Desc { get; set; }

        [Required]
        [Column("College_Year_Level_ID")]
        [MinLength(1)]
        [MaxLength(20)]
        public int College_Year_Level_ID { get; set; }
    }
}
