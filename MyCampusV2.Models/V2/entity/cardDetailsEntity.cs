using System;
using MyCampusV2.Models.V2.baseentity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_card_details")]
    public class cardDetailsEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Cardholder_ID")]
        public int Cardholder_ID { get; set; }

        [Column("Card_Serial")]
        [StringLength(50)]
        public string Card_Serial { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Column("Issued_Date")]
        public DateTime Issued_Date { get; set; }

        [Column("Expiry_Date")]
        public DateTime Expiry_Date { get; set; }

        //[Column("isActive")]
        //public bool isActive { get; set; }

        [Column("PAN")]
        [StringLength(50)]
        public string PAN { get; set; }

        [Column("UID")]
        public string Uid { get; set; }

        [Column("Remarks")]
        [StringLength(300)]
        public string Remarks { get; set; }

        [NotMapped]
        public string Card_Person_Type { get; set; }

        [NotMapped]
        public bool? Card_Is_Separated { get; set; }

        [NotMapped]
        public DateTime? Card_Separated_Date { get; set; }

        [ForeignKey("Person_ID")]
        public virtual personEntity PersonEntity { get; set; }
    }
}