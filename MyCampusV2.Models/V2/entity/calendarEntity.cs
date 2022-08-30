using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_calendar")]
    public class calendarEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Calendar_Code")]
        [StringLength(225)]
        public string Calendar_Code { get; set; }

        [Required]
        [Column("Title")]
        [StringLength(500)]
        public string Title { get; set; }

        [Column("Body")]
        [StringLength(3000)]
        public string Body { get; set; }

        [Column("Post_Date")]
        public DateTime Post_Date { get; set; }

        [Column("IsAll")]
        public bool IsAll { get; set; }

        [Column("IsSent")]
        public bool IsSent { get; set; }

        [Column("Year_Level")]
        [StringLength(3000)]
        public string Year_Level { get; set; }

        [NotMapped]
        public ICollection<recipients> Recipients { get; set; }
    }

    public class calendarRecipients
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
