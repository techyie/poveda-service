using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_fetcher_schedule")]
    public class fetcherScheduleEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Fetcher_Sched_ID")]
        public int Fetcher_Sched_ID { get; set; }

        [Required]
        [Column("Fetcher_ID")]
        public int Fetcher_ID { get; set; }

        [Required]
        [Column("Schedule_ID")]
        public int Schedule_ID { get; set; }

        [Required]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [ForeignKey("Schedule_ID")]
        public scheduleEntity ScheduleEntity { get; set; }
    }
}
