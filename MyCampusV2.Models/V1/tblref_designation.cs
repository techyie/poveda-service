using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_designation")]
    public class tblref_designation : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Designation_ID")]
        public int Designation_ID { get; set; }

        [Required]
        [Column("Designation_Name")]
        [StringLength(150)]
        public string Designation_Name { get; set; }

        //[Column("Position_ID")]
        //public int Position_ID { get; set; }

        [Column("Designation_Status")]
        [StringLength(10)]
        public string Designation_Status { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //public virtual ICollection<tblref_position> tblref_position { get; set; }
    }
}
