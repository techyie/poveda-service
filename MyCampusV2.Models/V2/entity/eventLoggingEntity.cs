using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_event_logging")]
    public class eventLoggingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EventLog_ID")]
        public int EventLog_ID { get; set; }

        [Column("User_ID")]
        [StringLength(50)]
        public int User_ID { get; set; }

        [Column("Form_ID")]
        public int Form_ID { get; set; }

        [Column("Source")]
        [StringLength(100)]
        public string Source { get; set; }

        [Column("Category")]
        [StringLength(50)]
        public string Category { get; set; }

        [Column("Log_Level")]
        public bool Log_Level { get; set; }

        [Column("Message")]
        [StringLength(300)]
        public string Message { get; set; }

        [Column("Log_Date_Time")]
        public DateTime Log_Date_Time { get; set; }

        [ForeignKey("Form_ID")]
        public virtual formEntity FormEntity { get; set; }

        [ForeignKey("User_ID")]
        public virtual userEntity UserEntity { get; set; }
    }
}