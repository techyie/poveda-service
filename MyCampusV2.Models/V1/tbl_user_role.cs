using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    //[Table("tbl_user_role")]
    public class tbl_user_role
    {
        [Key]
        public int ID { get; set; }

        public int User_ID { get; set; }

        public int Role_ID { get; set; }

        //[ForeignKey("User_ID")]
        //public virtual tbl_user tbl_user { get; set; }

        //[ForeignKey("Role_ID")]
        //public virtual tbl_roles tbl_roles { get; set; }

        //public virtual ICollection<tbl_user_role_access> tbl_user_role_access { get; set; }
    }
}
