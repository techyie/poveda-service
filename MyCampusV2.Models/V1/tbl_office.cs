using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_office")]
    public class tbl_office : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Office_ID")]
        public int Office_ID { get; set; }

        [Required]
        [Column("Office_Name")]
        [StringLength(150)]
        public string Office_Name { get; set; }

        [Column("Office_Status")]
        [StringLength(10)]
        public string Office_Status { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tblref_campus { get; set; }
    }
}
