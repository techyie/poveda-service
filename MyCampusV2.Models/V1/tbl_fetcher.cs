using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_fetcher")]
    public class tbl_fetcher : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Fetcher_ID")]
        public int Fetcher_ID { get; set; }

        public int Person_ID { get; set; }
        public int Connected_PersonID { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }

        //[ForeignKey("Connected_PersonID")]
        //public virtual tbl_person tbl_connected_person { get; set; }

        [Required]
        [Column("Relationship")]
        [StringLength(50)]
        public string Relationship { get; set; }
    }
}
