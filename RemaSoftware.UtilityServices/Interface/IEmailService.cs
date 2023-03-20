namespace RemaSoftware.UtilityServices.Interface
{
    public interface IEmailService
    {
        bool SendEmailForPasswordReset(string returnUrl, string email);
        bool SendEmailNewClientAccount(string email, string password);
        public bool SendEmailMissingPieces(string email, int missingPieces, int arrivedPieces, string DDT,
            string factoryName, string productSKU, string productName);
    }
}