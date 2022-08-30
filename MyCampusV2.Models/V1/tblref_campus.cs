using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_campus")]
    public class tblref_campus : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [Required]
        [Column("Campus_Name")]
        [StringLength(100)]
        public string Campus_Name { get; set; }

        [Column("Campus_Code")]
        [StringLength(10)]
        public string Campus_Code { get; set; }

        [Column("Campus_Address")]
        [StringLength(300)]
        public string Campus_Address { get; set; }

        [Column("Campus_ContactNo")]
        [StringLength(20)]
        public string Campus_ContactNo { get; set; }

        [Column("Division_ID")]
        public int Division_ID { get; set; }
    
        //[ForeignKey("Division_ID")]
        //public virtual tbl_division tbl_division {get;set;}

        //public virtual ICollection<tbl_area> tbl_area { get; set; }
        //public virtual ICollection<tbl_educ_level> tbl_educ_level { get; set; }
        //public virtual ICollection<tblref_department> tblref_department { get; set; }
        //public virtual ICollection<Visitor> visitor { get; set; }
    }
}
