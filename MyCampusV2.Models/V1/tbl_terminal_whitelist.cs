using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    [Table("tbl_terminal_whitelist")]
    public class tbl_terminal_whitelist : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Whitelist_ID")]
        public int Whitelist_ID { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Required]
        [Column("Date_Time_Uploaded")]
        public DateTime Date_Time_Uploaded { get; set; }

        //[ForeignKey("Terminal_ID")]
        //public virtual tbl_terminal tbl_terminal { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }
    }
}
