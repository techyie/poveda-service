using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_employee_type")]
    public class tbl_employee_type : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmpType_ID")]
        public int EmpType_ID { get; set; }

        [Column("EmpTypeDesc")]
        [StringLength(200)]
        public string EmpTypeDesc { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tblref_campus { get; set; }
    }
}
