using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Common.ViewModels.OfficeViewModel
{
    public class officeBatchUploadVM
    {
        [Column("CampusName")]
        public string CampusName { get; set; }
        [Column("OfficeName")]
        public string OfficeName { get; set; }
        //[Column("Office_Status")]
        //public string OfficeStatus { get; set; }
    }

    public class officeBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string OfficeName { get; set; }
        //public string OfficeStatus { get; set; }
    }

    public class officeBatchUploadHeadersVerifier
    {
        public System.Reflection.PropertyInfo CampusName { get; set; }
        public System.Reflection.PropertyInfo OfficeName { get; set; }
        //public System.Reflection.PropertyInfo OfficeStatus { get; set; }
    }
}
