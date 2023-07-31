using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.UtilityServices.Interface
{
    public interface IEmailService
    {
        bool SendEmailForPasswordReset(string returnUrl, string email);
        bool SendEmailNewClientAccount(string email, string password);
        public bool SendEmailMissingPieces(string email, int missingPieces, int arrivedPieces, string DDT,
            string factoryName, string productSKU, string productName);

        void SendEmailPrompt(List<string> email, string ddtCode, string note);
        public void SendEmailPriceVariation(string price, string mail, string message, string ddtCode, string cliente);
        public bool SendEmailStock(int id, string name, string sku, string supplierName, string email);

        public bool SendEmailAttendance(string period, string email, string attendance, byte[] pdfBytes);
        public void SendEmployeeAttendance(List<Employee> employees, List<string> email);
    }
}