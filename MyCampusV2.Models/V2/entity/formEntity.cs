using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_form")]
    public class formEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Form_ID")]
        public int Form_ID { get; set; }

        [Required]
        [Column("Form_Name")]
        [StringLength(300)]
        public string Form_Name { get; set; }

        [Required]
        [Column("Form_Description")]
        [StringLength(100)]
        public string Form_Description { get; set; }

        [Required]
        [Column("Form_Code")]
        [StringLength(125)]
        public string Form_Code { get; set; }

        [Column("Icon")]
        [StringLength(3000)]
        public string Icon { get; set; }

        [Column("IsTitle")]
        public bool IsTitle { get; set; }

        [Column("Parent_ID")]
        public int? Parent_ID { get; set; }

        [Column("Searchable")]
        public bool Searchable { get; set; }

        [Column("Administrator")]
        public bool Administrator { get; set; }

        [NotMapped]
        public List<formEntity> children = new List<formEntity>();

        public virtual ICollection<userAccessEntity> userAccessEntity { get; set; }
    }
}