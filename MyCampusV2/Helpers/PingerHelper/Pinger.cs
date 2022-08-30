using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Helpers
{
    public class Pinger
    {

        Logger log = new Logger();

        public bool ValidateTerminalConnection(string IP)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(IPAddress.Parse(IP));

                if (reply.Status == IPStatus.Success)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool TerminalConfigBuilder(terminalEntity terminal, terminalConfigurationEntity terminalconfig)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Configuration\"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Configuration\");

                Logger logger = new Logger();

                String filepath = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\mycampus.ini";

                logger.Activity(filepath);

                StringBuilder sb = new StringBuilder();

                /*
                 * FSUU VERSION
                 */
                //sb.Append("[Host1]" + "\n");
                //sb.Append("ip=" + terminalconfig. + "\n");
                //sb.Append("port = " + terminalconfig.Server_Port.ToString() + "\n");
                //sb.Append("\n");
                //sb.Append("[Host2]" + "\n");
                //sb.Append("ip=" + terminalconfig.Server_IP + "\n");
                //sb.Append("port = " + terminalconfig.Server_Port.ToString() + "\n");
                //sb.Append("\n");
                //sb.Append("[Viewer]" + "\n");
                //sb.Append("ip=" + terminalconfig.Viewer_IP + "\n");
                //sb.Append("port = " + terminalconfig.Viewer_Port.ToString() + "\n");
                //sb.Append("\n");
                //sb.Append("[Reader1]" + "\n");
                //sb.Append("sam_code = " + terminalconfig.Reader_Sam_1 + "\n");
                //sb.Append("direction = " + terminalconfig.Reader_Dir_1.ToString() + "\n");
                //sb.Append("enable_antipassback=" + (terminalconfig.Reader_Anti_Passback_1 == true ? "1" : "0") + "\n");
                //sb.Append("enable_vehicle_detector=" + (terminalconfig.Enable_Vehicle_Detector_1 == true ? "1" : "0") + "\n");
                //sb.Append("allow_write_direction=" + (terminalconfig.Allow_Write_Direction_1 == true ? "1" : "0") + "\n");
                //sb.Append("\n");
                //sb.Append("[Reader2]" + "\n");
                //sb.Append("sam_code = " + terminalconfig.Reader_Sam_2 + "\n");
                //sb.Append("direction = " + terminalconfig.Reader_Dir_2.ToString() + "\n");
                //sb.Append("enable_antipassback=" + (terminalconfig.Reader_Anti_Passback_2 == true ? "1" : "0") + "\n");
                //sb.Append("enable_vehicle_detector=" + (terminalconfig.Enable_Vehicle_Detector_2 == true ? "1" : "0") + "\n");
                //sb.Append("allow_write_direction=" + (terminalconfig.Allow_Write_Direction_1 == true ? "1" : "0") + "\n");
                //sb.Append("\n");
                //sb.Append("\n");
                //sb.Append("[Settings]" + "\n");
                //sb.Append("reserved1 =" + "**reserved**" + "\n");
                //sb.Append("schoolname = " + terminal.tbl_area.tbl_campus.Campus_Name + "\n");
                //sb.Append("terminal_code = " + terminal.Terminal_Code + "\n");
                //sb.Append("loop_delay = " + terminalconfig.Loop_Delay.ToString() + "\n");
                //sb.Append("turnstile_delay = " + terminalconfig.Turnstile_Delay.ToString() + "\n");
                //sb.Append("terminalid = " + terminal.ID + "\n");
                //sb.Append("terminal_sync_interval = " + terminalconfig.Sync_Interval.ToString() + "\n");
                //sb.Append("server_db = " + terminalconfig.Server_DB + "\n");
                //sb.Append("logviewer_db = " + terminalconfig.Viewer_DB + "\n");
                //sb.Append("allow_onhold=" + (terminalconfig.Allow_OnHold == true ? "1" : "0") + "\n");
                //sb.Append("terminal_command_interval = " + terminalconfig.Command_Interval.ToString() + "\n");
                //sb.Append("terminal_status = " + (terminal.IsActive == true ? "0" : "1") + "\n");

                /*
                 *  POVEDA
                 */
                sb.Append("[RESERVED 1]" + "\n");
                sb.Append("**reserved**" + "\n");
                sb.Append("[SCHOOL NAME]" + "\n");
                sb.Append(terminal.AreaEntity.CampusEntity.Campus_Name + "\n");
                sb.Append("[TERMINAL CODE]" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminal.Terminal_Code) + "\n");
                sb.Append("[Host Address 1]" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminalconfig.Host_IPAddress1) + "\n");
                sb.Append("[Host Port 1]" + "\n");
                sb.Append(terminalconfig.Host_Port1 + "\n");
                sb.Append("[Host Address 2]" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminalconfig.Host_IPAddress2) + "\n");
                sb.Append("[Host Port 2]" + "\n");
                sb.Append(terminalconfig.Host_Port2 + "\n");
                sb.Append("[Viewer Address]" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminalconfig.Viewer_Address) + "\n");
                sb.Append("[Viewer Port]" + "\n");
                sb.Append(terminalconfig.Viewer_Port + "\n");
                sb.Append("[Reader 1] (Set to 0 if no use)" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminalconfig.Reader_Name1) + "\n");
                sb.Append("[Direction 1] (Set to 0 if no use) 1 = left, 2 = right, 3=left only, 4=right only, 6=IN, 7=OUT" + "\n");     //** edl
                sb.Append(terminalconfig.Reader_Direction1.ToString() + "\n");

                sb.Append("[Enable antipassback for direction 1] (Set to 0 if no use else 1)" + "\n");
                sb.Append((terminalconfig.Enable_Antipassback1 == true ? "1" : "0") + "\n");
                sb.Append("[Reader 2] (Set to 0 if no use)" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminalconfig.Reader_Name2) + "\n");

                sb.Append("[Direction 2] (Set to 0 if no use) 1 = left, 2 = right, 3=left only, 4=right only, 6=IN, 7=OUT" + "\n");      //** edl
                sb.Append(terminalconfig.Reader_Direction2.ToString() + "\n");

                sb.Append("[Enable antipassback for direction 2] (Set to 0 if no use else 1)" + "\n");
                sb.Append((terminalconfig.Enable_Antipassback2 == true ? "1" : "0") + "\n");
                sb.Append("[Loop Delay] (Value in milliseconds)" + "\n");
                sb.Append(terminalconfig.Loop_Delay.ToString() + "\n");
                sb.Append("[Turnstile Delay] (value in milliseconds)" + "\n");
                sb.Append(terminalconfig.Turnstile_Delay.ToString() + "\n");
                sb.Append("[Terminal ID]" + "\n");
                sb.Append(terminal.Terminal_ID.ToString() + "\n");
                sb.Append("[Server DB]" + "\n");
                sb.Append("mcc_timekeeping_sppc" + "\n");
                sb.Append("[Terminal Sync Interval] (value in second)" + "\n");
                sb.Append(terminalconfig.Terminal_Sync_Interval.ToString() + "\n");

                sb.Append("[LogViewer DB]" + "\n");
                sb.Append(Encryption.Encryption.Decrypt(terminalconfig.Viewer_DB) + "\n");

                File.WriteAllText(filepath, String.Empty);
                File.AppendAllText(filepath, sb.ToString());
                sb.Clear();

                var connectionInfo = new ConnectionInfo(Encryption.Encryption.Decrypt(terminal.Terminal_IP), "pi", new PasswordAuthenticationMethod("pi", "AllcardTech2k15$pi"));
                using (var sftp = new SftpClient(connectionInfo))
                {
                    sftp.Connect();
                    sftp.ChangeDirectory("/home/pi/mycampus");
                    using (var uplfileStream = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"Configuration\mycampus.ini"))
                    {
                        sftp.UploadFile(uplfileStream, "mycampus.ini", true);
                    }
                    sftp.Disconnect();
                }

                return true;
            }
            catch (Exception e)
            {
                log.Activity(e.Message);
                return false;
            }
        } 
    }
}
