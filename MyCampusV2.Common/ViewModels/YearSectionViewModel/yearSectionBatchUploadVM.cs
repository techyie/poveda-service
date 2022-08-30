using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class yearSectionBatchUploadVM
    {
        [Column("Campus_Name")]
        public string CampusName { get; set; }
        [Column("Educ_Level_Name")]
        public string EducationalLevelName { get; set; }
        [Column("YearSec_Name")]
        public string YearSecName { get; set; }
    }

    public class yearSectionBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string YearSecName { get; set; }
    }
}
