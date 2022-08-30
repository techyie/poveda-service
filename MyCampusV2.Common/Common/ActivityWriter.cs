using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyCampusV2.Common
{
    public class ActivityWriter
    {
        public void Activity(string text)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Trail\"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Trail\");
                string logfile = AppDomain.CurrentDomain.BaseDirectory + @"Trail\" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";

                using (StreamWriter writer = new StreamWriter(logfile, true))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception err) { }
        }
    }
}
