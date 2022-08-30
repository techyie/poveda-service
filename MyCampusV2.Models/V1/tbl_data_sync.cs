using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_data_sync")]
    public class tbl_data_sync : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("UID")]
        [StringLength(8)]
        public string UID { get; set; }

        [Required]
        [Column("Card_Serial")]
        [StringLength(16)]
        public string Card_Serial { get; set; }

        [Required]
        [Column("Expiry_Date")]
        public DateTime Expiry_Date { get; set; }

        [Required]
        [Column("Action")]
        [StringLength(1)]
        public string Action { get; set; }

        [Required]
        [Column("On_Hold")]
        public bool On_Hold { get; set; }

        [Required]
        [Column("Blocked")]
        public bool Blocked { get; set; }

        [Required]
        [Column("Is_Upload")]
        public bool Is_Upload { get; set; }


        [Required]
        [Column("Terminal_ID")]
        [MinLength(1)]
        [MaxLength(20)]
        public int Terminal_ID { get; set; }

        [Required]
        [Column("Person_Type")]
        [StringLength(1)]
        public string Person_Type { get; set; }

        //[ForeignKey("Card_Serial")]
        //public virtual tbl_card_details tbl_card_details { get; set; }
    }
}
