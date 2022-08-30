using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_daily_logs")]
    public class dailyLogsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Log_ID")]
        public int Log_ID { get; set; }

        [Column("CardHolderID")]
        public int CardHolderID { get; set; }

        [Column("EffectivityDate")]
        public DateTime Log_Date { get; set; }

        [Column("DS_Action")]
        public bool Log_Status { get; set; }

        [Column("Log_Message")]
        [StringLength(300)]
        public string Log_Message { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("SMS_Unsent")]
        public bool SMS_Unsent { get; set; }

        [Column("SMS_Tries")]
        public int SMS_Tries { get; set; }

        [ForeignKey("CardHolderID")]

        public virtual cardDetailsEntity CardDetailsEntity { get; set; }

        [ForeignKey("Terminal_ID")]
        public virtual terminalEntity TerminalEntity { get; set; }
    }
}