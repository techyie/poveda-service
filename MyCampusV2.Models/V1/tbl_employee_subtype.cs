using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_employee_subtype")]
    public class tbl_employee_subtype : IBaseEntity
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

        //[ForeignKey("EmpType_ID")]
        //public virtual tbl_employee_type tbl_employee_type { get; set; }
    }
}
