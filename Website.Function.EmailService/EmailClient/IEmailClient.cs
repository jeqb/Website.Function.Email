using System.Threading.Tasks;

namespace Website.Function.EmailService.EmailClient
{
    public interface IEmailClient
    {
        public Task SendEmailAsync(string fromEmail, string toEmail, string subject, string htmlBody);
    }
}
