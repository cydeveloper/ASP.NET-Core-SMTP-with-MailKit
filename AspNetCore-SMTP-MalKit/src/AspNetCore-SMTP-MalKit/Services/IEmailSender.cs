using System.Threading.Tasks;

namespace SmtpTest.Services
{
    public interface IEmailSender 
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
