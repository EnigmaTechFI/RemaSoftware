namespace EmailService
{
    public interface IEmailService
    {
        bool SendEmailForPasswordReset(string returnUrl, string email);
    }
}