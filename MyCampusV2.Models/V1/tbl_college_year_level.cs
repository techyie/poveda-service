using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_college_year_level")]
    public class tbl_college_year_level : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(125)]
        [Column("College_Year_Name")]
        public string College_Year_Name { get; set; }

        [StringLength(125)]
        [Column("College_Year_Desc")]
        public string College_Year_Desc { get; set; }

        [Required]
        [Column("College_Course_ID")]
        [MinLength(1)]
        [MaxLength(20)]
        public int College_Course_ID { get; set; }
    }
}
