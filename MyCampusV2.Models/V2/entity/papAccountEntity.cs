using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_pap_acct")]
    public class papAccountEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("First_Name")]
        [StringLength(125)]
        public string First_Name { get; set; }

        [Column("Middle_Name")]
        [StringLength(100)]
        public string Middle_Name { get; set; }

        [Required]
        [Column("Last_Name")]
        [StringLength(125)]
        public string Last_Name { get; set; }

        [Required]
        [Column("Email_Address")]
        [StringLength(125)]
        public string Email_Address { get; set; }

        [Required]
        [Column("Mobile_Number")]
        [StringLength(13)]
        public string Mobile_Number { get; set; }

        [Required]
        [Column("Username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("Password")]
        [StringLength(250)]
        public string Password { get; set; }

        [Required]
        [Column("ConstUsername")]
        [StringLength(50)]
        public string ConstUsername { get; set; }

        [Column("Account_Code")]
        [StringLength(50)]
        public string Account_Code { get; set; }

        [Column("IsPending")]
        public bool IsPending { get; set; }

        [Column("IsRequestChangePassword")]
        public bool IsRequestChangePassword { get; set; }

        [Column("Linked_Students")]
        [StringLength(1000)]
        public string Lkd_Students { get; set; }

        [NotMapped]
        public ICollection<students> Linked_Students { get; set; }
    }

    public class students
    {
        public string key { get; set; }
        public string value { get; set; }
    }

}
