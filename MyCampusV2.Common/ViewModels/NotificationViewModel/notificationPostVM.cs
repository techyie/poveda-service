using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class notificationPostVM
    {
        public long? Notification_ID { get; set; }
        public string Message { get; set; }
        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public long Terminal_ID { get; set; }
    }
}
