namespace RemaSoftware.UtilityServices
{
    public interface IEmailService
    {
        bool SendEmailForPasswordReset(string returnUrl, string email);
    }
}