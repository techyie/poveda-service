using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models
{
    //[Table("tbl_year_level")]
    public class tblref_year_level : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Year_Level_Name")]
        [StringLength(125)]
        public string Year_Level_Name { get; set; }

        [Column("Year_Level_Desc")]
        [StringLength(125)]
        public string Year_Level_Desc { get; set; }

        [Column("Educ_Level_ID")]
        public int Educ_Level_ID { get; set; }

        //[ForeignKey("Educ_Level_ID")]
        //public virtual tbl_educ_level tbl_educ_level { get; set; }

        //public virtual ICollection<tblref_section> tblref_section { get; set; }
    }
}
