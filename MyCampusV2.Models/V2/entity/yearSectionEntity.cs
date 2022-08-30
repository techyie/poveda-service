using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_year_section")]
    public class yearSectionEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("YearSec_ID")]
        public int YearSec_ID { get; set; }

        [Required]
        [Column("YearSec_Name")]
        [StringLength(150)]
        public string YearSec_Name { get; set; }

        /*
         * Year Section doesn't have status, so I just put a property only with no annotation.
         * Just turn on the annotation if you want to use this
         */
        //[Column("YearSec_Status")]
        //[StringLength(10)]
        //public string YearSec_Status { get; set; }

        [Column("Level_ID")]
        public int Level_ID { get; set; }

        [ForeignKey("Level_ID")]
        public virtual educationalLevelEntity EducationalLevelEntity { get; set; }

        //[ForeignKey("Year_Level_ID")]
        //public educationalLevelEntity EducationalLevelEntity { get; set; }

        //public virtual ICollection<educationalLevelEntity> EducationalLevelList { get; set; }
    }
}