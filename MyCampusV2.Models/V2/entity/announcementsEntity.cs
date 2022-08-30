using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_announcements")]
    public class announcementsEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Announcement_Code")]
        [StringLength(225)]
        public string Announcement_Code { get; set; }

        [Required]
        [Column("Title")]
        [StringLength(500)]
        public string Title { get; set; }

        [Column("Body")]
        [StringLength(3000)]
        public string Body { get; set; }

        [Column("Date_Sent")]
        public DateTime Date_Sent { get; set; }

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

    public class recipients
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
