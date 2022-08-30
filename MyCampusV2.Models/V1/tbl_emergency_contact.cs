using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_emergency_contact")]
    public class tbl_emergency_contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmerCon_ID")]
        public int EmerCon_ID { get; set; }
        
        [Required]
        [Column("Full_Name")]
        [StringLength(250)]
        public string Full_Name { get; set; }

        [Required]
        [Column("Address")]
        [StringLength(300)]
        public string Address { get; set; }

        [Required]
        [Column("Contact_Number")]
        [StringLength(20)]
        public string Contact_Number { get; set; }

        [Required]
        [Column("Connected_PersonID")]
        public int Connected_PersonID { get; set; }

        [Required]
        [Column("Relationship")]
        [StringLength(50)]
        public string Relationship { get; set; }

        [Column("isActive")]
        public bool isActive { get; set; }

        //[ForeignKey("Connected_PersonID")]
        //public virtual tbl_person tbl_person { get; set; }


    }
}
