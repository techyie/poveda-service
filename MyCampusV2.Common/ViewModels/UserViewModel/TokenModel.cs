using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class TokenModel
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
        public long UserRole { get; set; }
        public long Person_ID { get; set; }
    }
}
