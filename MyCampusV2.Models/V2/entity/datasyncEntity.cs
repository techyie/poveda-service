using MyCampusV2.Models.V2.baseentity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_datasync")]
    public class datasyncEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DataSync_ID")]
        public int DataSync_ID { get; set; }

        [Column("Card_Serial")]
        [StringLength(50)]
        public string Card_Serial { get; set; }

        [Column("Expiry_Date")]
        public DateTime Expiry_Date { get; set; }

        [Column("DS_Action")]
        [StringLength(2)]
        public string DS_Action { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }

        [Column("Person_Type")]
        [StringLength(1)]
        public string Person_Type { get; set; }
    }
}