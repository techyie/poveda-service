using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_user_role")]
    public class userRoleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserRole_ID")]
        public int UserRole_ID { get; set; }

        [Required]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [Required]
        [Column("Role_ID")]
        public int Role_ID { get; set; }

        //[ForeignKey("User_ID")]
        //public virtual userEntity UserEntity { get; set; }

        //[ForeignKey("Role_ID")]
        //public virtual roleEntity RoleEntity { get; set; }

        //public virtual ICollection<userEntity> UserEntityList { get; set; }

        //public virtual ICollection<roleEntity> RoleEntityList { get; set; }
    }
}
