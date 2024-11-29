using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Service
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string receiptor, string subject, string body)
        {
            try
            {
                var email = Environment.GetEnvironmentVariable("SERVER_EMAIL");
                var password = Environment.GetEnvironmentVariable("SERVER_EMAIL_PASSWORD");
                var host = Environment.GetEnvironmentVariable("SMTP_HOST");
                var port = Environment.GetEnvironmentVariable("SMTP_PORT");

                if (
                    string.IsNullOrEmpty(email)
                    || string.IsNullOrEmpty(password)
                    || string.IsNullOrEmpty(host)
                    || string.IsNullOrEmpty(port)
                )
                {
                    throw new Exception(
                        "Email server configuration not found. Ensure the .env file is correctly configured and placed in the root directory."
                    );
                }

                var client = new SmtpClient(host, Convert.ToInt32(port));
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(email, password);

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(email);
                mailMessage.To.Add(receiptor);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
