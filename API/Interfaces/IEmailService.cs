namespace API.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiptor, string subject, string body);
    }
}
