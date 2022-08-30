using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class studentSectionBatchUploadVM
    {
        [Column("campus_Name")]
        public string CampusName { get; set; }
        [Column("educ_Level_Name")]
        public string EducationalLevelName { get; set; }
        [Column("year_Level_Name")]
        public string YearLevelName { get; set; }
        [Column("section_Name")]
        public string SectionName { get; set; }
        [Column("start_Time")]
        public string StartTime { get; set; }
        [Column("end_Time")]
        public string EndTime { get; set; }
        [Column("half_Day")]
        public string HalfDay { get; set; }
        [Column("grace_Period")]
        public string GracePeriod { get; set; }
    }

    public class studentSectionBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string YearLevelName { get; set; }
        public string SectionName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string HalfDay { get; set; }
        public string GracePeriod { get; set; }
    }
    public class studentSectionBatchUploadHeadersVerifier
    {
        public System.Reflection.PropertyInfo CampusName { get; set; }
        public System.Reflection.PropertyInfo EducationalLevelName { get; set; }
        public System.Reflection.PropertyInfo YearLevelName { get; set; }
        public System.Reflection.PropertyInfo SectionName { get; set; }
        public System.Reflection.PropertyInfo StartTime { get; set; }
        public System.Reflection.PropertyInfo EndTime { get; set; }
        public System.Reflection.PropertyInfo HalfDay { get; set; }
        public System.Reflection.PropertyInfo GracePeriod { get; set; }
    }
}
