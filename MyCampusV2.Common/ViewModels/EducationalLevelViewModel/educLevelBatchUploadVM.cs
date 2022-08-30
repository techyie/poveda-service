using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class educLevelBatchUploadVM
    {
        [Column("Campus_Name")]
        public string CampusName { get; set; }
        [Column("Level_Name")]
        public string EducationalLevelName { get; set; }
        [Column("Level_Status")]
        public string EducationalLevelStatus { get; set; }
        [Column("hasCourse")]
        public string College { get; set; }
    }

    public class educLevelBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string EducationalLevelStatus { get; set; }
        public string College { get; set; }
    }

    public class educLevelBatchUploadHeadersVerifier
    {
        public System.Reflection.PropertyInfo CampusName { get; set; }
        public System.Reflection.PropertyInfo EducationalLevelName { get; set; }
        public System.Reflection.PropertyInfo EducationalLevelStatus { get; set; }
        public System.Reflection.PropertyInfo isCollege { get; set; }
    }
}
