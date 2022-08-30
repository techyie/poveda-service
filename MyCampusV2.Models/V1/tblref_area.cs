using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tblref_area")]
    public class tblref_area : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Area_ID")]
        public int Area_ID { get; set; }

        [Required]
        [Column("Area_Name")]
        [StringLength(100)]
        public string Area_Name { get; set; }

        [Required]
        [Column("Area_Description")]
        [StringLength(300)]
        public string Area_Description { get; set; }

        [Required]
        [Column("Area_Status")]
        [StringLength(10)]
        public string Area_Status { get; set; }

        [Required]
        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tbl_campus { get; set; }

        //public virtual ICollection<tbl_terminal> tbl_terminal { get; set; }
    }
}
