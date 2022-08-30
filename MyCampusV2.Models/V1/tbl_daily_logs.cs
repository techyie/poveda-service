using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_daily_logs")]
    public class tbl_daily_logs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Log_ID")]
        public int Log_ID { get; set; }

        [Required]
        [Column("CardHolderID")]
        public int CardHolderID { get; set; }

        [Required]
        [Column("Log_Date")]
        public DateTime Log_Date { get; set; }

        [Required]
        [Column("Log_Status")]
        public int Log_Status { get; set; }

        [Required]
        [Column("Log_Message")]
        [StringLength(300)]
        public string Log_Message { get; set; }

        [Required]
        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        //[ForeignKey("CardHolderID")]
        //public virtual tbl_card_details tbl_card_details { get; set; }

        //[ForeignKey("Terminal_ID")]
        //public virtual tbl_terminal tbl_terminal { get; set; }
        
    }
}
