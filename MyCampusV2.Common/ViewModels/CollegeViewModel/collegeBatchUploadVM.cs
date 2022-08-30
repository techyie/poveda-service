using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class collegeBatchUploadVM
    {
        //public long campus_Id { get; set; }
        [Column("campus_Name")]
        public string CampusName { get; set; }
        //public long educ_Level_Id { get; set; }
        [Column("educ_Level_Name")]
        public string EducationalLevelName { get; set; }
        [Column("college_Name")]
        public string CollegeName { get; set; }
        [Column("college_Desc")]
        public string CollegeDescription { get; set; }
    }

    public class collegeBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string CollegeName { get; set; }
        public string CollegeDescription { get; set; }
    }
}
