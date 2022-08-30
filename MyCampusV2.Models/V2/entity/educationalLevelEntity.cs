using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_educ_level")]
    public class educationalLevelEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Level_ID")]
        public int Level_ID { get; set; }

        [Required]
        [Column("Level_Name")]
        [StringLength(100)]
        public string Level_Name { get; set; }

        [Column("Level_Status")]
        [StringLength(10)]
        public string Level_Status { get; set; }

        [Column("hasCourse")]
        public bool hasCourse { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [ForeignKey("Campus_ID")]
        public virtual campusEntity CampusEntity { get; set; }

        //public virtual ICollection<campusEntity> CampusList { get; set; }
    }
}