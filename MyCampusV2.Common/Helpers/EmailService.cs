using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Common.Helpers
{
    public class EmailService
    {
        public async Task<bool> SendMessage(string recipient, string subject, string body, string SenderName, string SenderEmail, string SenderUsername, string SenderPassword, string MailServer, int MailPort)
        {
            try
            {
                var message = new MimeMessage();
                message.Subject = subject;
                message.From.Add(new MailboxAddress(SenderName, SenderEmail));
                message.To.Add(new MailboxAddress(recipient, recipient));

                var builder = new BodyBuilder();
                builder.HtmlBody = string.Format(body);
                message.Body = builder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    //client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(MailServer, MailPort, true);
                    client.Authenticate(SenderUsername, SenderPassword);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }

                return true;

            }
            catch (Exception ex)
            {
                //using (var log = new LoggerConfiguration().WriteTo.File("logs\\SendMessage\\Logs.txt", rollingInterval: RollingInterval.Day).CreateLogger())
                //{ log.Error("SendMessage" + String.Format("-- ** {0} **", ex.Message)); }
                Console.WriteLine(ex.Message);
            }

            return false;

        }

        //public async Task Send(string emailHost, int emailPort, string mailUsername, string mailPassword, string[] args)
        //{
        //    var message = new MimeMessage();

        //    message.From.Add(new MailboxAddress("ATLAS", mailusername));

        //    message.Subject = args[1];

        //    var bodyBuilder = new BodyBuilder();
        //    bodyBuilder.HtmlBody = args[2];

        //    message.Body = bodyBuilder.ToMessageBody();

        //    using (var client = new SmtpClient())
        //    {
        //        IPHostEntry hostInfo = Dns.GetHostEntry(emailHost);
        //        client.Connect(hostInfo.HostName, emailPort, true);

        //        // Note: only needed if the SMTP server requires authentication
        //        client.Authenticate(mailusername, mailpassword);

        //        await client.SendAsync(message);
        //        client.Disconnect(true);
        //    }

        //}
    }
}
