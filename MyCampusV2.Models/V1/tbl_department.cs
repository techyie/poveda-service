using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_department")]
    public class tbl_department : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Department_Name")]
        [StringLength(125)]
        public string Department_Name { get; set; }

        [Column("Department_Desc")]
        [StringLength(125)]
        public string Department_Desc { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tblref_campus { get; set; }

        //public virtual ICollection<tblref_position> tblref_position { get; set; }
    }
}
