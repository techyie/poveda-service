using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_card_details")]
    public class tbl_card_details : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Cardholder_ID")]
        public int Cardholder_ID { get; set; }
        
        [Required]
        [Column("Card_Serial")]
        [StringLength(50)]
        public string Card_Serial { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }
        
        [Column("Issued_Date")]
        public DateTime Issued_Date { get; set; }

        [Required]
        [Column("Expiry_Date")]
        public DateTime Expiry_Date { get; set; }
        
        [Column("PAN")]
        [StringLength(50)]
        public string PAN { get; set; }
        
        [Column("Remarks")]
        [StringLength(300)]
        public string Remarks { get; set; }

        [ForeignKey("Person_ID")]
        public virtual tbl_person tbl_person { get; set; }
        
        public virtual ICollection<tbl_terminal_whitelist> tbl_terminal_whitelist { get; set; }
    }
}
