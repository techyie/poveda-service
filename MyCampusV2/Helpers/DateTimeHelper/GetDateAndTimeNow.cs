using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Helpers.DateTimeHelper
{
    public static class GetDateAndTimeNow
    {
        public static DateTime GetNow(string destinationTimeZoneId)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, destinationTimeZoneId);
        }
    }
}
