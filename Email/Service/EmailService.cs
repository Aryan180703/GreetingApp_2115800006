using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using Email.Interface;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Email.Service
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSSL;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SMTPServer"];
            _smtpPort = int.Parse(configuration["EmailSettings:SMTPPort"]);
            _smtpUsername = configuration["EmailSettings:SMTPUsername"];
            _smtpPassword = configuration["EmailSettings:SMTPPassword"];
            _enableSSL = bool.Parse(configuration["EmailSettings:EnableSSL"]);
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = _enableSSL;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}