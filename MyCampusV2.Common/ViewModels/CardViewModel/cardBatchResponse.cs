using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class cardBatchResponse
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public ICollection<FailedDeatils> Failed { get; set; }

        public class FailedDeatils
        {
            public object card { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
