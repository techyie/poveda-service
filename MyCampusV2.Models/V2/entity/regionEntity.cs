using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{ 
    [Table("tbl_region")]
    public class regionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Name")]
        [StringLength(225)]
        public string Name { get; set; }

        [Column("Code")]
        [StringLength(30)]
        public string Code { get; set; }
        
        public virtual ICollection<divisionEntity> DivisionList { get; set; }
    }
}