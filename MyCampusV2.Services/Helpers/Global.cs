using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Helpers
{
    public static class Global
    {
        public static bool ValidateStr(string chars)
        {
            var withoutSpecial = new string(chars.Where(c => Char.IsLetterOrDigit(c)
                                                || Char.IsWhiteSpace(c)
                                                || c.Equals('ñ') || c.Equals('_')
                                                || c.Equals('-') || c.Equals('\'')).ToArray());
            if (chars != withoutSpecial)
            {
                return false; // has special characters
            }
            return true;
        }
    }
}
