using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_user")]
    public class tbl_user : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("User_Name")]
        [StringLength(100)]
        public string User_Name { get; set; }

        [Required]
        [Column("User_Password")]
        [StringLength(100)]
        public string User_Password { get; set; }

        [Column("Person_Type")]
        [StringLength(1)]
        public string Person_Type { get; set; }

        [Column("IsAdmin")]
        public bool IsAdmin { get; set; }

        [Column("IsGuard")]
        public bool IsGuard { get; set; }

        [NotMapped]
        public string Token { get; set; }

        //public virtual tbl_user_role tbl_user_roles { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }
    }
}
