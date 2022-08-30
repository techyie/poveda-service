using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    [Table("tbl_terminal_whitelist_count")]
    public class terminalWhitelistCount
    {
        [Key]
        [Column("Count")]
        public int Count { get; set; }

    }
}
