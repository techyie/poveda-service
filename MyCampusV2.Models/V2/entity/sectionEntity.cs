using MyCampusV2.Models.V2.baseentity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_section")]
    public class sectionEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Section_ID")]
        public long Section_ID { get; set; }

        [Required]
        [Column("Section_Name")]
        [StringLength(125)]
        public string Section_Name { get; set; }

        [Column("Year_Level_ID")]
        public int Year_Level_ID { get; set; }

        [ForeignKey("Year_Level_ID")]
        public virtual yearLevelEntity YearLevelEntity { get; set; }
    }
}
