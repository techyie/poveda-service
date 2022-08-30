
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_person_employee")]
    public class tbl_person_employee : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        public int Position_ID { get; set; }
        public int Person_ID { get; set; }
        public bool IsTeaching { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }

        //[ForeignKey("Position_ID")]
        //public virtual tblref_position tblref_position { get; set; }
    }
}
