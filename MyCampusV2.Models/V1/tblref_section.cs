using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_section")]
    public class tblref_section : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Section_Name")]
        [StringLength(125)]
        public string Section_Name { get; set; }

        [Column("Section_Desc")]
        [StringLength(125)]
        public string Section_Desc { get; set; }

        [Column("Year_Level_ID")]
        public int Year_Level_ID { get; set; }

        //[ForeignKey("Year_Level_ID")]
        //public virtual tblref_year_level tblref_year_level { get; set; }
    }
}
