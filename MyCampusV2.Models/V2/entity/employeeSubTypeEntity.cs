using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_employee_subtype")]
    public class employeeSubTypeEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmpSubtype_ID")]
        public int EmpSubtype_ID { get; set; }

        [Required]
        [Column("EmpSubTypeDesc")]
        [StringLength(200)]
        public string EmpSubTypeDesc { get; set; }

        [Column("EmpType_ID")]
        public int EmpType_ID { get; set; }

        [ForeignKey("EmpType_ID")]
        public virtual empTypeEntity EmployeeType { get; set; }

        //public virtual ICollection<employeeTypeEntity> EmployeeTypeList { get; set; }
    }
}