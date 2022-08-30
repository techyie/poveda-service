using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class employeeTableVM
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
        public string telephoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string employeeTypeDesc { get; set; }
        public int employeeTypeId { get; set; }
        public string employeeSubTypeDesc { get; set; }
        public int employeeSubTypeId { get; set; }
        public string positionName { get; set; }
        public int positionId { get; set; }
        public int departmentId { get; set; }
        public string departmentName { get; set; }
        public string campusName { get; set; }
        public int? campusId { get; set; }
        public string status { get; set; }

        public string emergencyFullname { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyRelationship { get; set; }
        public string emergencyAddress { get; set; }

        public string sss { get; set; }
        public string pagibig { get; set; }
        public string tin { get; set; }
        public string philhealth { get; set; }
    }
}
