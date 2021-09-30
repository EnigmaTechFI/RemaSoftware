using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace UtilityServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public bool SendEmailForPasswordReset(string returnUrl, string email)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(new MailAddress(email));
 
                mailMessage.Subject = "Reset Password";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = returnUrl;
 
                SmtpClient client = new SmtpClient();
                var mailPwd = _configuration["EmailConfig:Password"];
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential(mailAddressSender, mailPwd);
                client.Host = _configuration["EmailConfig:SmtpServer"];
                client.Port = int.Parse(_configuration["EmailConfig:Port"]);
                client.EnableSsl = true;
                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    // todo log errore invio mail
                }
            }
            catch (Exception e)
            {
                // todo log
            }
            return false;
        }
    }
}