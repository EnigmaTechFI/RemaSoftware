namespace RemaSoftware.UtilityServices.Interface
{
    public interface IEmailService
    {
        bool SendEmailForPasswordReset(string returnUrl, string email);
        bool SendEmailNewClientAccount(string email, string password);
        public bool SendEmailMissingPieces(string email, int missingPieces, int arrivedPieces, string DDT,
            string factoryName, string productSKU, string productName);

        void SendEmailPrompt(string email, string ddtCode);
        public void SendEmailPriceVariation(decimal price, string mail, string message, string ddtCode, string cliente);
    }
}