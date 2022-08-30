using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    [Table("tbl_terminal_configuration")]
    public class tbl_terminal_configuration : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Config_ID")]
        public int Config_ID { get; set; }

        [Required]
        [Column("TerminalID_Uploaded")]
        public int TerminalID_Uploaded { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Terminal_Schedule")]
        public string Terminal_Schedule { get; set; }
        
        [Required]
        [StringLength(50)]
        [Column("School_Name")]
        public string School_Name { get; set; }

        [Required]
        [StringLength(3000)]
        [Column("Terminal_Code")]
        public string Terminal_Code { get; set; }

        [Required]
        [StringLength(3000)]
        [Column("Host_IPAddress1")]
        public string Host_IPAddress1 { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Host_Port1")]
        public string Host_Port1 { get; set; }

        [Required]
        [StringLength(3000)]
        [Column("Host_IPAddress2")]
        public string Host_IPAddress2 { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Host_Port2")]
        public string Host_Port2 { get; set; }

        [Required]
        [StringLength(3000)]
        [Column("Viewer_Address")]
        public string Viewer_Address { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Viewer_Port")]
        public string Viewer_Port { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Reader_Name1")]
        public string Reader_Name1 { get; set; }

        [Required]
        [StringLength(1)]
        [Column("Reader_Direction1")]
        public string Reader_Direction1 { get; set; }

        [Required]
        [Column("Enable_Antipassback1")]
        public bool Enable_Antipassback1 { get; set; }

        [Required]
        [StringLength(3000)]
        [Column("Reader_Name2")]
        public string Reader_Name2 { get; set; }

        [Required]
        [StringLength(1)]
        [Column("Reader_Direction2")]
        public string Reader_Direction2 { get; set; }

        [Required]
        [Column("Enable_Antipassback2")]
        public bool Enable_Antipassback2 { get; set; }

        [Required]
        [Column("Loop_Delay")]
        public string Loop_Delay { get; set; }

        [Required]
        [Column("Turnstile_Delay")]
        public bool Turnstile_Delay { get; set; }

        [Required]
        [StringLength(3000)]
        [Column("Viewer_DB")]
        public bool Viewer_DB { get; set; }

        //[ForeignKey("TerminalID_Uploaded")]
        //public virtual tbl_terminal tbl_terminal { get; set; }
    }
}
