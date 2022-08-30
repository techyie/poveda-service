using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_college")]
    public class collegeEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("College_ID")]
        public int College_ID { get; set; }

        [Required]
        [Column("College_Name")]
        [StringLength(150)]
        public string College_Name { get; set; }

        [Column("College_Status")]
        [StringLength(10)]
        public string College_Status { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [Column("Level_ID")]
        public int Level_ID { get; set; }

        [ForeignKey("Level_ID")]
        public virtual educationalLevelEntity EducationalLevelEntity { get; set; }

        //public virtual ICollection<educationalLevelEntity> EducationalLevelList { get; set; }
    }
}