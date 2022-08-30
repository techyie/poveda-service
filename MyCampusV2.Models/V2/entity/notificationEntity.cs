using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_notification")]
    public class notificationEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Notification_ID")]
        public int Notification_ID { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("Notification_Message")]
        [StringLength(300)]
        public string Notification_Message { get; set; }

        [Column("Date_To_Display_From")]
        public DateTime Date_To_Display_From { get; set; }

        [Column("Date_To_Display_To")]
        public DateTime Date_To_Display_To { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        [Column("GUID")]
        [StringLength(100)]
        public string GUID { get; set; }

        [ForeignKey("Person_ID")]
        public personEntity PersonEntity { get; set; }

        [ForeignKey("Terminal_ID")]
        public terminalEntity TerminalEntity { get; set; }
    }
}