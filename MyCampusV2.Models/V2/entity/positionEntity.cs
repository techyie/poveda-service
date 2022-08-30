using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_position")]
    public class positionEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Position_ID")]
        public int Position_ID { get; set; }

        [Required]
        [Column("Position_Name")]
        [StringLength(150)]
        public string Position_Name { get; set; }

        [Column("Position_Status")]
        [StringLength(10)]
        public string Position_Status { get; set; }

        [Column("Department_ID")]
        public int Department_ID { get; set; }

        [ForeignKey("Department_ID")]
        public virtual departmentEntity DepartmentEntity { get; set; }

        //public virtual ICollection<departmentEntity> DepartmentList { get; set; }
    }
}