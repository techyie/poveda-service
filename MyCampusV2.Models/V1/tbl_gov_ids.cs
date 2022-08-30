using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_gov_ids")]
    public class tbl_gov_ids
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("SSS")]
        [StringLength(30)]
        public string SSS { get; set; }

        [Required]
        [Column("TIN")]
        [StringLength(30)]
        public string TIN { get; set; }
        
        [Required]
        [Column("PhilHealth")]
        [StringLength(30)]
        public string PhilHealth { get; set; }

        [Required]
        [Column("PAG_IBIG")]
        [StringLength(30)]
        public string PagIbig { get; set; }
        
        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }
    }
}
