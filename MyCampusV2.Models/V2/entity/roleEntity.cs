using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_role")]
    public class roleEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Role_ID")]
        public int Role_ID { get; set; }

        [Required]
        [Column("Role_Name")]
        [StringLength(125)]
        public string Role_Name { get; set; }

        public virtual ICollection<rolePermissionEntity> RolePermissionEntityList { get; set; }
        //public virtual ICollection<userRoleEntity> tbl_user_role { get; set; }
    }
}
