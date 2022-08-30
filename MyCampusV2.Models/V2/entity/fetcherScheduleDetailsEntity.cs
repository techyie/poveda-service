using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_fetcher_schedule_details")]
    public class fetcherScheduleDetailsEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Fetcher_Sched_Dtl_ID")]
        public int Fetcher_Sched_Dtl_ID { get; set; }

        [Required]
        [Column("Fetcher_Sched_ID")]
        public int Fetcher_Sched_ID { get; set; }

        [Required]
        [Column("Fetcher_Group_ID")]
        public int Fetcher_Group_ID { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [ForeignKey("Fetcher_Sched_ID")]
        public fetcherScheduleEntity FetcherScheduleEntity { get; set; }

        [ForeignKey("Fetcher_Group_ID")]
        public fetcherGroupEntity FetcherGroupEntity { get; set; }

        [ForeignKey("Person_ID")]
        public personEntity PersonEntity { get; set; }
    }
}
