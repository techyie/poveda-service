using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_department")]
    public class departmentEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Department_ID")]
        public int Department_ID { get; set; }

        [Required]
        [Column("Department_Name")]
        [StringLength(150)]
        public string Department_Name { get; set; }

        [Column("Department_Status")]
        [StringLength(10)]
        public string Department_Status { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [ForeignKey("Campus_ID")]
        public virtual campusEntity CampusEntity { get; set; }

        //public virtual ICollection<campusEntity> CampusList { get; set; }
    }
}