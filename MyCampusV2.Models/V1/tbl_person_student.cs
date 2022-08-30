using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_person_student")]
    public class tbl_person_student : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        public int Section_ID { get; set; }
       // public int Course_SectionID { get; set; }
        public int Person_ID { get; set; }
        public int? Course_ID {get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }

        //[ForeignKey("Section_ID")]
        //public virtual tblref_section tblref_section { get; set; }

        //[ForeignKey("Course_ID")]
        //public virtual tblref_course tbl_college_course { get; set; }


    }
}
