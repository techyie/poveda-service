using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{

    [Table("tbl_announcements_recipients")]
    public class announcementsRecipientsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Announcement_ID")]
        public int Announcement_ID { get; set; }

        [ForeignKey("Announcement_ID")]
        public virtual announcementsEntity AnnouncementsEntity { get; set; }

        [Required]
        [Column("Year_Level_ID")]
        public int Year_Level_ID { get; set; }

        [ForeignKey("Year_Level_ID")]
        public virtual yearSectionEntity YearSectionEntity { get; set; }
    }
}
