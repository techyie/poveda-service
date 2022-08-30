using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_schedule")]
    public class tbl_schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Schedule_ID")]
        public long Schedule_ID { get; set; }

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
        [StringLength(50)]
        public string Schedule_Time_From { get; set; }

        [Required]
        [Column("Schedule_Time_To")]
        [StringLength(50)]
        public string Schedule_Time_To { get; set; }

        [Required]
        [Column("Schedule_Status")]
        [StringLength(10)]
        public string Schedule_Status { get; set; }

        [Required]
        [Column("Date_Time_Added")]
        [StringLength(50)]
        public string Date_Time_Added { get; set; }

        [Required]
        [Column("Last_Updated")]
        [StringLength(50)]
        public string Last_Updated { get; set; }
    }
}
