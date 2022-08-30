using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    [Table("Audit_Trail_Report_Count")]
    public class Audit_Trail_Report_Count
    {
        [Key]
        [Column("Count")]
        public int Count { get; set; }
    }
}
