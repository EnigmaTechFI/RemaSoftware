using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NLog;

namespace UtilityServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                    Logger.Error(ex, "Errore durante l'invio della mail per il recupero della password.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante il processo di invio della mail per il recupero della password.");
            }
            return false;
        }
    }
}