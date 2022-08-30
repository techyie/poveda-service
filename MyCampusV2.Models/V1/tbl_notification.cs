using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyCampusV2.Models
{
    //[Table("tbl_notification")]
    public class tbl_notification : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Notification_ID")]
        public int Notification_ID { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [StringLength(300)]
        [Column("Notification_Message")]
        public string Notification_Message { get; set; }

        [Required]
        [Column("Date_To_Display_From")]
        public DateTime Date_To_Display_From { get; set; }

        [Required]
        [Column("Date_To_Display_To")]
        public DateTime Date_To_Display_To { get; set; }

        [Required]
        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Required]
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }

        //[ForeignKey("Terminal_ID")]
        //public virtual tbl_terminal tbl_terminal { get; set; }
        
    }
}
