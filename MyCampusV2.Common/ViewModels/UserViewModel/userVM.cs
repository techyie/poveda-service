using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Common.ViewModels
{
    public class userVM
    {
        public long? User_ID { get; set; }
        public long Role_ID { get; set; }
        public string Role_Name { get; set; }
        public string User_Name { get; set; }
        public string Id_Number { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string Department_Name { get; set; }
        public string Email_Address { get; set; }
        public string Status { get; set; }
        // public string User_Password { get; set; }    
	}

    public class userAuditTrailVM
    {
        public long? user_ID { get; set; }
        public string full_Info { get; set; }
    }
}
