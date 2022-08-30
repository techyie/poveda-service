using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_audit_trail")]
    public class tbl_audit_trail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

       public int Form_ID { get; set; }
       public int User_ID { get; set; }
       public string Action { get; set; }
       public DateTime Date { get;set; }

        //[ForeignKey("Form_ID")]
        //public virtual tbl_form tbl_form { get; set; }

        //[ForeignKey("User_ID")]
        //public virtual tbl_user tbl_user { get; set; }
    }
}
