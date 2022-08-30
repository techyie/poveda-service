using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class visitorPersonBatchUploadVM
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

        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
    }

    public class visitorPersonBatchUploadHeaders
    {
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
    }
}
