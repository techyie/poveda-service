using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    [Table("Report_Count")]
    public class Report_Count
    {
        [Key]
        [Column("Count")]
        public int Count { get; set; }
    }
}
