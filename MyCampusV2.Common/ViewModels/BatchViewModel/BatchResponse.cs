using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class BatchResponse
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public ICollection<FailedDeatils> Failed { get; set; }

        public class FailedDeatils
        {
            public object Item { get; set; }
            public string ErrorMessage { get; set; }
        }
    }

    public class BatchUploadResponse
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public string FileName { get; set; }
        public int ProcessCount { get; set; }
        public int TotalCount { get; set; }
        public string Status { get; set; }

        public ICollection<UploadDetails> Details { get; set; }

        public class UploadDetails
        {
            public object Item { get; set; }
            public string Message { get; set; }
            public bool isError { get; set; }
        }
    }

    //public class BatchProcessResponse
    //{
    //    public int ProcessCount { get; set; }
    //    public int TotalCount { get; set; }
    //    public string Status { get; set; }
    //    public string FileName { get; set; }

    //}
}
