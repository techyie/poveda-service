using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class courseBatchUploadVM
    {
        //public long campus_Id { get; set; }
        [Column("campus_Name")]
        public string CampusName { get; set; }
        //public long educ_Level_Id { get; set; }
        [Column("educ_Level_Name")]
        public string EducationalLevelName { get; set; }
        //public long college_Id { get; set; }
        [Column("college_Name")]
        public string CollegeName { get; set; }

        //public string course_Code { get; set; }
        [Column("course_Name")]
        public string CourseName { get; set; }
        [Column("course_Desc")]
        public string CourseDescription { get; set; }
    }

    public class courseBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string CollegeName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
    }
}
