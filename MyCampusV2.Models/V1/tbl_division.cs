using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_division")]
    public class tbl_division
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        
        [Required]
        [Column("RegionID")]
        public int RegionID { get; set; }

        [Required]
        [StringLength(125)]
        [Column("Name")]
        public string Name { get; set; }

        //[ForeignKey("RegionID")]
        //public virtual tbl_region tbl_region { get; set; }

        //public virtual ICollection<tblref_campus> tbl_campus { get; set; }
    }
}
