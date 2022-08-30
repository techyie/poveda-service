using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_employee_type")]
    public class empTypeEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmpType_ID")]
        public int EmpType_ID { get; set; }

        //[Required]
        [Column("EmpTypeDesc")]
        [StringLength(200)]
        public string EmpTypeDesc { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [ForeignKey("Campus_ID")]
        public virtual campusEntity CampusEntity { get; set; }

        //public virtual ICollection<campusEntity> CampustList { get; set; }
    }
}