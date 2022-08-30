using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_terminal_whitelist")]
    public class terminalWhitelistEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Whitelist_ID")]
        public int Whitelist_ID { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("Date_Time_Uploaded")]
        public DateTime Date_Time_Uploaded { get; set; }
    }
}