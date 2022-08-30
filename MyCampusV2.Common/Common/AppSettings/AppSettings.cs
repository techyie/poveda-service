using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Common
{
    /*The app settings class contains properties defined in the appsettings.json file and is used for accessing application settings via objects that injected into classes using the ASP.NET Core built in dependency injection.*/

    public class AppSettings
    {
        public string Secret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }

    //public class AdSettings
    //{

    //}
}
