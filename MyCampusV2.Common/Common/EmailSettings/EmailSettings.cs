using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string EmailTitle { get; set; }
        public string ResetPasswordTitle { get; set; }
        public string Logo { get; set; }
        public string Template { get; set; }
        public string ResetPasswordTemplate { get; set; }
        public string ActivationUrl { get; set; }
        public string LinkValidationUrl { get; set; }
    }
}
