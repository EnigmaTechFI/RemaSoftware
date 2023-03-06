namespace RemaSoftware.UtilityServices.Interface
{
    public interface IEmailService
    {
        bool SendEmailForPasswordReset(string returnUrl, string email);
        bool SendEmailNewClientAccount(string email, string password);
    }
}