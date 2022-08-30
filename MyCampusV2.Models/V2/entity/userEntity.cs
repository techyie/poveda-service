using MyCampusV2.Models.V2.baseentity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_user")]
    public class userEntity : povedaBaseEntity
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

        [Required]
        [Column("Role_ID")]
        public int Role_ID { get; set; }

        [ForeignKey("Person_ID")]
        public virtual personEntity PersonEntity { get; set; }

        [ForeignKey("Role_ID")]
        public virtual roleEntity RoleEntity { get; set; }

        //[NotMapped]
        //public string Token { get; set; }
    }
}
