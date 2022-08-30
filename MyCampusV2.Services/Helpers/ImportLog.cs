using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyCampusV2.Services.Helpers
{
    public class ImportLog
    {
        public void Logging(string path, string filename, string text)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (StreamWriter writer = new StreamWriter(path + filename, true))
                {
                    writer.WriteLine(text);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception err) { }
        }

        public void WriteError(Exception error)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Error\"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Error\");
                string logfile = AppDomain.CurrentDomain.BaseDirectory + @"Error\" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";

                using (StreamWriter sw = new StreamWriter(logfile, true))
                {
                    sw.WriteLine("Time: " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
                    sw.WriteLine("Source: " + error.Source + Environment.NewLine);
                    sw.WriteLine("Message: " + error.Message + Environment.NewLine);
                    sw.WriteLine("Stack Trace: " + error.StackTrace + Environment.NewLine);
                    sw.WriteLine("Target Site: " + error.TargetSite + Environment.NewLine);
                    sw.WriteLine("Data: " + error.Data + Environment.NewLine);
                    sw.WriteLine("--------------------------------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                }
            }
            catch { }
        }
    }
}
