using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_role_permission")]
    public class rolePermissionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Permission_ID")]
        public int Permission_ID { get; set; }

        [Required]
        [Column("Role_ID")]
        public int Role_ID { get; set; }

        [Column("Can_Access")]
        public bool Can_Access { get; set; }

        [Column("Can_Insert")]
        public bool Can_Insert { get; set; }

        [Column("Can_Update")]
        public bool Can_Update { get; set; }

        [Column("Can_Delete")]
        public bool Can_Delete { get; set; }

        [Required]
        [Column("Form_ID")]
        public int Form_ID { get; set; }

        [ForeignKey("Form_ID")]
        public virtual formEntity FormEntity { get; set; }

        [ForeignKey("Role_ID")]
        public virtual roleEntity RoleEntity { get; set; }
    }
}