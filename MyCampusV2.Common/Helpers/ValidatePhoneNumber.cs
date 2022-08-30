using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MyCampusV2.Common.Helpers
{
    public static class ValidatePhoneNumber
    {
        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+[0-9]{9})$").Success;
        }
    }
}
