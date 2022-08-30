using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_office")]
    public class officeEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Office_ID")]
        public int Office_ID { get; set; }

        [Required]
        [Column("Office_Name")]
        [StringLength(150)]
        public string Office_Name { get; set; }

        [Column("Office_Status")]
        [StringLength(10)]
        public string Office_Status { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [ForeignKey("Campus_ID")]
        public virtual campusEntity CampusEntity { get; set; }

        //public virtual ICollection<campusEntity> CampusList { get; set; }
    }
}