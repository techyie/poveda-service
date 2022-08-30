using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_audit_trail")]
    public class auditTrailEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Form_ID")]
        public int Form_ID { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }

        [Column("Action")]
        [StringLength(700)]
        public string Action { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [ForeignKey("Form_ID")]
        public virtual formEntity FormEntity { get; set; }

        [ForeignKey("User_ID")]
        public virtual userEntity UserEntity { get; set; }
    }
}
