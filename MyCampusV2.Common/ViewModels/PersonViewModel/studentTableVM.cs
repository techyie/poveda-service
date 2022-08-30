using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class studentTableVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string address { get; set; }
        public string contactNumber { get; set; }
        public string emailAddress { get; set; }
        public string educLevelName { get; set; }
        public int educLevelId { get; set; }
        public int yearSecId { get; set; }
        public string yearSecName { get; set; }
        public int studSecId { get; set; }
        public int? courseId { get; set; }
        public string courseName { get; set; }
        public int? collegeId { get; set; }
        public string collegeName { get; set; }
        public string description { get; set; }
        public string campusName { get; set; }
        public int? campusId { get; set; }
        public string dateEnrolled { get; set; }
        public string emergencyFullname { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyRelationship { get; set; }
        public string emergencyAddress { get; set; }

        public string isDropOut { get; set; }
        public string dropOutCode { get; set; }
        public string isTransferred { get; set; }
        public string transferredSchoolName { get; set; }
        public string dropOutOtherRemark { get; set; }
        public string isTransferredIn { get; set; }
        public string transferredInSchoolName { get; set; }
    }

}
