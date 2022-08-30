using MyCampusV2.Models.V2.entity;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.Helpers
{
    public class ErrorLogging
    {
        public void Error(string function, string text)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Error\"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Error\");
                string logfile = AppDomain.CurrentDomain.BaseDirectory + @"Error\" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";

                using (StreamWriter writer = new StreamWriter(logfile, true))
                {
                    writer.WriteLine(string.Format(
                        "FUNCTION: " + function + '\n' + 
                        "MESSAGE: " + text + '\n' + 
                        "---------------------------------------------------", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception) { }
        }

        public void Write_Message(string message)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Logs\"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Logs\");
                string logfile = AppDomain.CurrentDomain.BaseDirectory + @"Logs\" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";

                using (StreamWriter sw = new StreamWriter(logfile, true))
                {
                    sw.WriteLine("Time: " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
                    sw.WriteLine("Message: " + message + Environment.NewLine);
                    sw.WriteLine("--------------------------------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                }
            }
            catch { }
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
