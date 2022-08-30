using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models
{
    //[Table("tblref_year_section")]
    public class tblref_year_section : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("YearSec_ID")]
        public int YearSec_ID { get; set; }

        [Required]
        [Column("YearSec_Name")]
        [StringLength(150)]
        public string YearSec_Name { get; set; }
        
        [Column("Level_ID")]
        public int Level_ID { get; set; }

        //[ForeignKey("Level_ID")]
        //public virtual tblref_educ_level tblref_educ_level { get; set; }
        
    }
}
