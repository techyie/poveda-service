using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_division")]
    public class divisionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Name")]
        [StringLength(125)]
        public string Name { get; set; }

        [Column("Region_ID")]
        public int Region_ID { get; set; }

        [ForeignKey("Region_ID")]
        public virtual regionEntity RegionEntity { get; set; }
        
    }
}