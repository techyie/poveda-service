using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{

    [Table("tbl_calendar_recipients")]
    public class calendarRecipientsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Calendar_ID")]
        public int Calendar_ID { get; set; }

        [ForeignKey("Calendar_ID")]
        public virtual calendarEntity CalendarEntity { get; set; }

        [Required]
        [Column("Year_Level_ID")]
        public int Year_Level_ID { get; set; }

        [ForeignKey("Year_Level_ID")]
        public virtual yearSectionEntity YearSectionEntity { get; set; }
    }
}
