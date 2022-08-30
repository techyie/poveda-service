using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    [Table("Alarm_Report_Count")]
    public class Alarm_Report_Count
    {
        [Key]
        [Column("Count")]
        public int Count { get; set; }
    }
}
