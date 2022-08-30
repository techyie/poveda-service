using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_user_access")]
    public class tbl_user_access
    {
        [Key]
        public int Access_ID { get; set; }
        //public int User_ID { get; set; }
        public int Form_ID { get; set; }
        public bool Can_Access { get; set; }
        public bool Can_Insert { get; set; }
        public bool Can_Update { get; set; }
        public bool Can_Delete { get; set; }

        public int Role_ID { get; set; }

        //[ForeignKey("Form_ID")]
        //public tbl_form tbl_form { get; set; }
        //[ForeignKey("User_ID")]
        //public tbl_form tbl_user { get; set; }
    }
}
