using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Interface;
using Product = It.FattureInCloud.Sdk.Model.Product;

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
        
        public bool SendEmailNewAccount(string email, string password)
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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione riguardo i pezzi.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante l'invio della mail per la comunicazione riguardo i pezzi.");
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
                for (int i = email.Count - 1; i >= 0; i--)
                {
                    if (email[i] == "lorenzo.vettori11@gmail.com")
                    {
                        email.RemoveAt(i);
                    }
                }
                
                mailMessage.To.Add(new MailAddress(email[0]));
                for (int i = 1; i < email.Count; i++)
                {
                    mailMessage.CC.Add(email[i]);
                }
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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione riguardole ddt.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante l'invio della mail per la comunicazione riguardo le ddt.");
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
        
        public bool SendEmailAttendance(string period, string email, string attendance, byte[] pdfBytes)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(new MailAddress(email));
 
                mailMessage.Subject = "Resoconto Presenze Mensili";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/mail-attendance.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();
                str.Close();
                
                Attachment attachment = new Attachment(attendance);
                mailMessage.Attachments.Add(attachment);
                
                Attachment attachment1 = new Attachment(new MemoryStream(pdfBytes), "Presenze_" +period+".pdf", "application/pdf");
                mailMessage.Attachments.Add(attachment1);

                
                MailText = MailText.Replace("[period]", period);

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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione delle timbrature.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante il processo di invio della mail per la comunicazione delle timbrature.");
            }
            return false;
        }
        
        public bool SendEmailNoAttendance(Employee employee, string note, List<string> email)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                for (int i = email.Count - 1; i >= 0; i--)
                {
                    if (email[i] == "lorenzo.vettori11@gmail.com")
                    {
                        email.RemoveAt(i);
                    }
                }
                
                mailMessage.To.Add(new MailAddress(email[0]));
                for (int i = 1; i < email.Count; i++)
                {
                    mailMessage.CC.Add(email[i]);
                }
                mailMessage.CC.Add(employee.Mail);
                mailMessage.Subject = "Resoconto mancata timbratura";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/mail-no-attendance.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();
                str.Close();
                
                MailText = MailText.Replace("[employee]", employee.Name + " " + employee.Surname);
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
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione delle timbrature.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante il processo di invio della mail per la comunicazione delle timbrature.");
            }
            return false;
        }
        
        public bool SendEmailNoPrice(string product, List<string> operations, string image, List<string> email)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                for (int i = email.Count - 1; i >= 0; i--)
                {
                    if (email[i] == "lorenzo.vettori11@gmail.com")
                    {
                        email.RemoveAt(i);
                    }
                }
                
                mailMessage.To.Add(new MailAddress(email[0]));
                for (int i = 1; i < email.Count; i++)
                {
                    mailMessage.CC.Add(email[i]);
                }
                mailMessage.Subject = "Promemoria prezzo assente";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/mail-no-price.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();
                str.Close();
                
                MailText = MailText.Replace("[product]", product);
                
                string operationList = operations[0].ToString();
                for(int i = 1; i < operations.Count; i++)
                {
                    operationList = operationList + "<br/>" + operations[i];
                }
                
                MailText = MailText.Replace("[operation]", operationList);
                
                if (!string.IsNullOrEmpty(image))
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            byte[] imageData = wc.DownloadData(image);
                            string attachmentName = Guid.NewGuid().ToString() + ".png";
                            Attachment attachment = new Attachment(new MemoryStream(imageData), attachmentName, "image/png");
                            attachment.ContentDisposition.Inline = true;
                            attachment.ContentId = attachmentName;
                            mailMessage.Attachments.Add(attachment);
                            MailText = MailText.Replace("[image]", $"<img src='cid:{attachment.ContentId}' style='width:300px;'>");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Errore durante il download e l'allegato dell'immagine: " + ex.Message);
                    }
                }
                
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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione riguardo al prezzo.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante il processo di invio della mail la comunicazione riguardo al prezzo.");
            }
            return false;
        }
        
        public bool SendEmailNoAttendanceEmployee(Employee employee, string email)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                mailMessage.To.Add(email);
                mailMessage.Subject = "Resoconto mancata timbratura";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/mail-no-attendance-employee.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();
                str.Close();
                
                MailText = MailText.Replace("[employee]", employee.Name + " " + employee.Surname);
                string login =  _configuration["ApplicationUrl"] + "Account/Login";
                MailText = MailText.Replace("[login]", login);
                
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
                    Logger.Error(ex, "Errore durante l'invio della mail per la comunicazione delle timbrature.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante il processo di invio della mail per la comunicazione delle timbrature.");
            }
            return false;
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
                    Logger.Error(ex, "Errore durante l'invio della mail per il controllo del magazzino.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante il processo di invio della mail per il controllo del magazzino.");
            }

            return false;
        }

        public void SendEmployeeAttendance(List<Employee> employees, List<string> email)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var mailAddressSender = _configuration["EmailConfig:EmailAddress"];
                mailMessage.From = new MailAddress(mailAddressSender);
                
                for (int i = email.Count - 1; i >= 0; i--)
                {
                    if (email[i] == "lorenzo.vettori11@gmail.com")
                    {
                        email.RemoveAt(i);
                    }
                }
                
                mailMessage.To.Add(new MailAddress(email[0]));
                for (int i = 1; i < email.Count; i++)
                {
                    mailMessage.CC.Add(email[i]);
                }
                
                mailMessage.Subject = "Resoconto mancate timbrature";
                mailMessage.IsBodyHtml = true;
                string FilePath = "wwwroot/MailTemplate/attendance-notify.html";  
                StreamReader str = new StreamReader(FilePath);  
                string MailText = str.ReadToEnd();  
                str.Close();

                string tmpName = null;
                foreach(var item in employees)
                {
                    tmpName = tmpName + item.Name + " " + item.Surname + " </br> ";
                }
                MailText = MailText.Replace("[Impiegati]", tmpName);  

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
    }
}