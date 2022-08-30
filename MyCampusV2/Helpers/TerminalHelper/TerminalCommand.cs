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
    public class TerminalCommand
    {
        Logger log = new Logger();

        public bool RebootCommand(terminalEntity terminal)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(Encryption.Encryption.Decrypt(terminal.Terminal_IP), "pi", new PasswordAuthenticationMethod("pi", "AllcardTech2k15$pi"));
                using (var client = new SshClient(connectionInfo))
                {
                    client.Connect();
                    client.RunCommand("sudo shutdown -r 1");
                    client.Disconnect();
                }

                log.Activity("Successfully Rebooted Terminal " + terminal.Terminal_Name);
                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }

        public bool ShutdownCommand(terminalEntity terminal)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(Encryption.Encryption.Decrypt(terminal.Terminal_IP), "pi", new PasswordAuthenticationMethod("pi", "AllcardTech2k15$pi"));
                using (var client = new SshClient(connectionInfo))
                {
                    client.Connect();
                    //client.RunCommand("sudo shutdown -h now");
                    client.RunCommand("sudo shutdown -h 1");
                    client.Disconnect();
                }

                log.Activity("Successfully Shutdown Terminal " + terminal.Terminal_Name);
                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }


    }

}
