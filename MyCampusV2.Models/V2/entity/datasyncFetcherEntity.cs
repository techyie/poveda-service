using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_datasync_fetcher")]
    public class datasyncFetcherEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("FetcherSerial")]
        [StringLength(50)]
        public string FetcherSerial { get; set; }

        [Column("StudentSerial")]
        [StringLength(50)]
        public string StudentSerial { get; set; }

        [Column("SchedDays")]
        [StringLength(50)]
        public string SchedDays { get; set; }

        [Column("TimeFrom")]
        public TimeSpan TimeFrom { get; set; }

        [Column("TimeTo")]
        public TimeSpan TimeTo { get; set; }

        [Column("DS_Action")]
        [StringLength(1)]
        public string DS_Action { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("StudID")]
        [StringLength(50)]
        public string StudID { get; set; }

        [Column("FetcherID")]
        [StringLength(50)]
        public string FetcherID { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }
    }
}