using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_school_calendar")]
    public class schoolCalendarEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int? School_Calendar_ID { get; set; }

        [Required]
        [Column("School_Year")]
        public string School_Year { get; set; }

        [Column("Year")]
        public int Year { get; set; }

        [Column("Month")]
        public int Month { get; set; }
        
        [Column("Days")]
        public string Days { get; set; }
    }
}
