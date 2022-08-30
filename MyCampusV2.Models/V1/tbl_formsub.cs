using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    //[Table("tbl_sub_form")]
    public class tbl_formsub
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Sub_Form_Name")]
        public string Form_Name { get; set; }
        [Column("Sub_Form_Desc")]
        public string Form_Description { get; set; }
        [Column("Sub_Form_Code")]
        public string Form_Code { get; set; }
        [Column("Url")]
        public string url { get; set; }
        [Column("Icon")]
        public string icon { get; set; }

        public tbl_formsub()
        {
         //   user_role_access = new HashSet<tbl_user_role_access>();
        }

        //[ForeignKey("Form_ID")]
        //public virtual tbl_form tbl_form { get; set; }
        //public ICollection<tbl_user_role_access> user_role_access { get; set; }
    }
}
