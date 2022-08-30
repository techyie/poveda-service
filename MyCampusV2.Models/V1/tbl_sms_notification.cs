using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyCampusV2.Models
{
    [Table("tbl_sms_notification")]
    public class tbl_sms_notification : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Notification_ID { get; set; }

        public string Message { get; set; }
        //public bool IsActive { get; set; }
        //public long? Added_By { get; set; }
        //public DateTime Date_Time_Added { get; set; }
        //public long? Updated_By { get; set; }
        //public DateTime Last_Updated { get; set; }
    }
}
