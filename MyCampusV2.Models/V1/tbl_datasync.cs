using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_datasync")]
    public class tbl_datasync : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DataSync_ID")]
        public int DataSync_ID { get; set; }

        [Required]
        [Column("Card_Serial")]
        [StringLength(50)]
        public string Card_Serial { get; set; }

        [Required]
        [Column("Expiry_Date")]
        public DateTime Expiry_Date { get; set; }

        [Required]
        [Column("DS_Action")]
        [StringLength(2)]
        public string DS_Action { get; set; }

        [Required]
        [Column("Terminal_ID")]
        [MaxLength(11)]
        public int Terminal_ID { get; set; }

        [Required]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [Required]
        [Column("Person_Type")]
        [StringLength(1)]
        public string Person_Type { get; set; }

        //[ForeignKey("Card_Serial")]
        //public virtual tbl_card_details tbl_card_details { get; set; }
    }
}
