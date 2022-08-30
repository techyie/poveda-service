using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_area")]
    public class tbl_area : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Area_Name")]
        [StringLength(125)]
        public string Area_Name { get; set; }

        [Column("Area_Desc")]
        [StringLength(125)]
        public string Area_Desc { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tbl_campus { get; set; }

        //public virtual ICollection<tbl_terminal> tbl_terminal { get; set; }
    }
}
