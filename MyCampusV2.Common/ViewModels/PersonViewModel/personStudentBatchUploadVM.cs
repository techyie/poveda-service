using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class personStudentBatchUploadVM
    {
        //public long? personID { get; set; }
        //public string idNumber { get; set; }
        //public string firstName { get; set; }
        //public string middleName { get; set; }
        //public string lastName { get; set; }
        //public string birthdate { get; set; }
        //public string gender { get; set; }
        //public string contactNumber { get; set; }
        //public string emailAddress { get; set; }
        //public string address { get; set; }
        //public string campusName { get; set; }
        //public string educlevelName { get; set; }
        //public string yearlevelName { get; set; }
        //public string sectionName { get; set; }
        //public string collegeName { get; set; }
        //public string courseName { get; set; }

        //public string emergencyFullname { get; set; }
        //public string emergencyContact { get; set; }
        //public string emergencyRelationship { get; set; }
        //public string emergencyAddress { get; set; }

        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string YearLevelName { get; set; }
        public string SectionName { get; set; }
        public string CollegeName { get; set; }
        public string CourseName { get; set; }
        public string DateEnrolled { get; set; }

        public string EmergencyFullname { get; set; }
        public string EmergencyContactNo { get; set; }
        public string EmergencyRelationship { get; set; }
        public string EmergencyAddress { get; set; }

        public bool IsCollegeTemplate { get; set; }

        public string IsDropOut { get; set; }
        public string DropOutCode { get; set; }
        public string IsTransferred { get; set; }
        public string TransferredSchoolName { get; set; }
        public string DropOutOtherRemark { get; set; }
        public string IsTransferredIn { get; set; }
        public string TransferredInSchoolName { get; set; }
    }

    public class personStudentBatchUploadHeaders
    {
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string CampusName { get; set; }
        public string EducationalLevelName { get; set; }
        public string YearLevelName { get; set; }
        public string SectionName { get; set; }
        public string CollegeName { get; set; }
        public string CourseName { get; set; }
        public string DateEnrolled { get; set; }

        public string EmergencyFullname { get; set; }
        public string EmergencyContactNo { get; set; }
        public string EmergencyRelationship { get; set; }
        public string EmergencyAddress { get; set; }

        public string IsDropOut { get; set; }
        public string DropOutCode { get; set; }
        public string IsTransferred { get; set; }
        public string TransferredSchoolName { get; set; }
        public string DropOutOtherRemark { get; set; }
        public string IsTransferredIn { get; set; }
        public string TransferredInSchoolName { get; set; }
    }
}
