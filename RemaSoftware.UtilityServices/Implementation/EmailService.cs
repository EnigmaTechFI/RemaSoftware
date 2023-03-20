using System;
using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.UtilityServices.Interface;

namespace RemaSoftware.UtilityServices.Implementation
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
                string FilePath = "wwwroot/MailTemplate/reset-password.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();  
                str.Close();
                
                MailText = MailText.Replace("[LinkResetPassword]", returnUrl);  
                
                mailMessage.Body =  MailText;

                SmtpClient client = new SmtpClient();
                var mailPwd = _configuration["EmailConfig:Password"];
                
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
        
        public bool SendEmailNewClientAccount(string email, string password)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(new MailAddress(email));
 
                mailMessage.Subject = "Credenziali nuovo account";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/mail-new-account.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();  
                str.Close();  
                
                MailText = MailText.Replace("[email]", email);  
                MailText = MailText.Replace("[password]", password);  
                
                mailMessage.Body =  MailText;
                

                SmtpClient client = new SmtpClient();
                var mailPwd = _configuration["EmailConfig:Password"];
                //client.UseDefaultCredentials = true; Commentato perchè non prende le credenziali della mail se setto a true
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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione dell'account.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante l'invio della mail per la comunicazione dell'account.");
            }
            return false;
        }
        
        public bool SendEmailMissingPieces(string email, int missingPieces, int arrivedPieces, string DDT, string factoryName, string productSKU, string productName)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(new MailAddress(email));
 
                mailMessage.Subject = "Pezzi mancanti DDT N° " + DDT;
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/pezzi-mancanti.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();  
                str.Close();  
                
                MailText = MailText.Replace("[Azienda]", factoryName);  
                MailText = MailText.Replace("[CodiceDDT]", DDT);  
                MailText = MailText.Replace("[ProdottoSKU]", productSKU);  
                MailText = MailText.Replace("[ProdottoNAME]", productName);  
                MailText = MailText.Replace("[Mancanti]", missingPieces.ToString());  
                MailText = MailText.Replace("[Arrivati]", arrivedPieces.ToString());  
                
                mailMessage.Body =  MailText;
                

                SmtpClient client = new SmtpClient();
                var mailPwd = _configuration["EmailConfig:Password"];
                //client.UseDefaultCredentials = true; Commentato perchè non prende le credenziali della mail se setto a true
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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione dell'account.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante l'invio della mail per la comunicazione dell'account.");
            }
            return false;
        }
    }

}