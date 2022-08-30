using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_department")]
    public class tblref_department : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Department_ID")]
        public int Department_ID { get; set; }

        [Required]
        [Column("Department_Name")]
        [StringLength(150)]
        public string Department_Name { get; set; }

        [Column("Department_Status")]
        [StringLength(10)]
        public string Department_Status { get; set; }

        [Required]
        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tblref_campus { get; set; }

        //public virtual ICollection<tblref_position> tblref_position { get; set; }
    }
}
