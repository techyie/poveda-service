using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Common.ViewModels.UserViewModel
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool HasRole { get; set; }
        public bool ValidLogin { get; set; }
        public string mail { get; set; }
    }
}
