using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_terminal_configuration")]
    public class terminalConfigurationEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Config_ID")]
        public int Config_ID { get; set; }

        [Column("TerminalID_Uploaded")]
        public int TerminalID { get; set; }

        [Column("Terminal_Schedule")]
        [StringLength(50)]
        public string Terminal_Schedule { get; set; }

        [Column("School_Name")]
        [StringLength(100)]
        public string School_Name { get; set; }

        [Column("Terminal_Code")]
        [StringLength(2000)]
        public string Terminal_Code { get; set; }

        [Column("Host_IPAddress1")]
        [StringLength(1000)]
        public string Host_IPAddress1 { get; set; }

        [Column("Host_Port1")]
        [StringLength(50)]
        public string Host_Port1 { get; set; }

        [Column("Host_IPAddress2")]
        [StringLength(2000)]
        public string Host_IPAddress2 { get; set; }

        [Column("Host_Port2")]
        [StringLength(10)]
        public string Host_Port2 { get; set; }

        [Column("Viewer_Address")]
        [StringLength(2000)]
        public string Viewer_Address { get; set; }

        [Column("Viewer_Port")]
        [StringLength(10)]
        public string Viewer_Port { get; set; }

        [Column("Reader_Name1")]
        [StringLength(2000)]
        public string Reader_Name1 { get; set; }

        [Column("Reader_Direction1")]
        public int Reader_Direction1 { get; set; }

        [Column("Enable_Antipassback1")]
        public bool Enable_Antipassback1 { get; set; }

        [Column("Reader_Name2")]
        [StringLength(2000)]
        public string Reader_Name2 { get; set; }

        [Column("Reader_Direction2")]
        public int Reader_Direction2 { get; set; }

        [Column("Enable_Antipassback2")]
        public bool Enable_Antipassback2 { get; set; }

        [Column("Loop_Delay")]
        public int Loop_Delay { get; set; }

        [Column("Turnstile_Delay")]
        public int Turnstile_Delay { get; set; }

        [Column("Terminal_Sync_Interval")]
        public int Terminal_Sync_Interval { get; set; }

        [Column("Viewer_DB")]
        [StringLength(2000)]
        public string Viewer_DB { get; set; }

        [Column("Date_Time_Uploaded")]
        public DateTime Date_Time_Uploaded { get; set; }

        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Column("Server_DB")]
        public string Server_DB { get; set; }

        [ForeignKey("Terminal_ID")]
        public virtual terminalEntity TerminalEntity { get; set; }
    }
}