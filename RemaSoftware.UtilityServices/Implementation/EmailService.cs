using System;
using System.Collections.Generic;
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
                string login =  _configuration["ApplicationUrl"] + "Account/Login";
                
                
                MailText = MailText.Replace("[email]", email);  
                MailText = MailText.Replace("[password]", password);
                MailText = MailText.Replace("[login]", login);
                
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
                string login =  _configuration["ApplicationUrl"] + "Account/Login";

                
                MailText = MailText.Replace("[Azienda]", factoryName);  
                MailText = MailText.Replace("[CodiceDDT]", DDT);  
                MailText = MailText.Replace("[ProdottoSKU]", productSKU);  
                MailText = MailText.Replace("[ProdottoNAME]", productName);  
                MailText = MailText.Replace("[Mancanti]", missingPieces.ToString());  
                MailText = MailText.Replace("[Arrivati]", arrivedPieces.ToString());  
                MailText = MailText.Replace("[login]", login);

                
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

        public void SendEmailPrompt(List<string> email, string ddtCode, string note)
        {
           try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(new MailAddress(email[0]));
                for(int i = 1; i< email.Count; i++)
                    mailMessage.CC.Add(email[i]);
                mailMessage.Subject = "Sollecito DDT N° " + ddtCode;
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/sollecito-ddt.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();  
                str.Close();  
                string login =  _configuration["ApplicationUrl"] + "Account/Login";

                
                MailText = MailText.Replace("[CodiceDDT]", ddtCode);  
                MailText = MailText.Replace("[login]", login);
                MailText = MailText.Replace("[note]", note);

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
        }

        public void SendEmailPriceVariation(string price, string mail, string message, string ddtCode, string cliente)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(new MailAddress(mail));
                mailMessage.Subject = "Richiesta variazione prezzo DDT N° " + ddtCode;
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/variazione-prezzo.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();  
                str.Close();  
                MailText = MailText.Replace("[CodiceDDT]", ddtCode);  
                MailText = MailText.Replace("[CorpoMessaggio]", message);  
                MailText = MailText.Replace("[Cliente]", cliente);  
                MailText = MailText.Replace("[PrezzoNuovo]", price);  
                mailMessage.Body =  MailText;
                SmtpClient client = new SmtpClient();
                var mailPwd = _configuration["EmailConfig:Password"];
                client.Credentials = new System.Net.NetworkCredential(mailAddressSender, mailPwd);
                client.Host = _configuration["EmailConfig:SmtpServer"];
                client.Port = int.Parse(_configuration["EmailConfig:Port"]);
                client.EnableSsl = true;
                client.Send(mailMessage);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante l'invio della mail per variazione prezzo DDT.");
                throw new Exception("Errore durante l&#39;invio della mail per variazione prezzo DDT.");
            }
        }

        public bool SendEmailStock(int id, string name, string sku, string supplierName, string mail)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                
                mailMessage.To.Add(new MailAddress(mail));

                mailMessage.Subject = "Prodotto magazzino sotto limite scorte";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/under-limit.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[id]", id.ToString());
                MailText = MailText.Replace("[name]", name);
                MailText = MailText.Replace("[supplierName]", supplierName);
                MailText = MailText.Replace("[sku]", sku);



                mailMessage.Body = MailText;

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
    }

}