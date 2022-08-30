using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_user_access")]
    public class userAccessEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Access_ID")]
        public int Access_ID { get; set; }

        [Required]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [Required]
        [Column("Form_ID")]
        public int Form_ID { get; set; }

        [Required]
        [Column("Permission_ID")]
        public int Permission_ID { get; set; }

        [Column("Can_Access")]
        public bool Can_Access { get; set; }

        [Column("Can_Insert")]
        public bool Can_Insert { get; set; }

        [Column("Can_Update")]
        public bool Can_Update { get; set; }

        [Column("Can_Delete")]
        public bool Can_Delete { get; set; }

        [ForeignKey("Form_ID")]
        public virtual formEntity FormEntity { get; set; }

        [ForeignKey("User_ID")]
        public virtual userEntity UserEntity { get; set; }

        [ForeignKey("Permission_ID")]
        public virtual rolePermissionEntity RolePermissionEntity { get; set; }
    }
}