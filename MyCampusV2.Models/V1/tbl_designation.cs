using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_designation")]
    public class tbl_designation : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Required]
        [Column("Designation_Name")]
        [StringLength(150)]
        public string Designation_Name { get; set; }

        [Column("Designation_Status")]
        [StringLength(10)]
        public string Designation_Status { get; set; }

        [Column("Campus_ID")]
        public long Campus_ID { get; set; }

        //public virtual ICollection<tblref_position> tblref_position { get; set; }
    }
}
