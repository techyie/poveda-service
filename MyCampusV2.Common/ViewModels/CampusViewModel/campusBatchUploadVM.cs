using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class campusBatchUploadVM
    {
        //public long campus_Id { get; set; }
        [Column("campus_code")]
        public string CampusCode { get; set; }
        [Column("campus_name")]
        public string CampusName { get; set; }
        [Column("campus_status")]
        public string CampusStatus { get; set; }
        [Column("campus_address")]
        public string CampusAddress { get; set; }
        [Column("campus_contactNo")]
        public string CampusContactNo { get; set; }
        [Column("region")]
        public string Region { get; set; }
        [Column("division")]
        public string Division { get; set; }
    }

    public class campusBatchUploadHeaders
    {
        public string CampusCode { get; set; }
        public string CampusName { get; set; }
        public string CampusAddress { get; set; }
        public string Region { get; set; }
        public string Division { get; set; }
    }
}
