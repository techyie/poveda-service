using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    [Table("tbl_schedule")]
    public class scheduleEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Schedule_ID")]
        public int Schedule_ID { get; set; }

        [Required]
        [Column("Schedule_Name")]
        [StringLength(200)]
        public string Schedule_Name { get; set; }

        [Required]
        [Column("Schedule_Days")]
        [StringLength(50)]
        public string Schedule_Days { get; set; }

        [Required]
        [Column("Schedule_Time_From")]
        public TimeSpan Schedule_Time_From { get; set; }

        [Required]
        [Column("Schedule_Time_To")]
        public TimeSpan Schedule_Time_To { get; set; }

        [Required] 
        [Column("Schedule_Status")]
        [StringLength(10)]
        public string Schedule_Status { get; set; }

    }
}
