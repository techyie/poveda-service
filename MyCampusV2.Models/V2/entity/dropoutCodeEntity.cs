using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_dropout_code")]
    public class dropoutCodeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Code")]
        [StringLength(10)]
        public string Code { get; set; }

        [Column("Name")]
        [StringLength(125)]
        public string Name { get; set; }

        [Column("Description")]
        [StringLength(225)]
        public string Description { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}