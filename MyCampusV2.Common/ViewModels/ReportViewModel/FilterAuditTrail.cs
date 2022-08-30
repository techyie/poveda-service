using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class FilterAuditTrail
    {
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public int user_ID { get; set; }
        public int form_ID { get; set; }
    }
}
