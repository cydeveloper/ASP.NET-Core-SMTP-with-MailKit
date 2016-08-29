using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace SmtpTest.Services
{
    public class AuthMessageSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("TEST SMTP", "Enter_Sender_EmailAdress"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            await Task.Run(() =>
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.live.com", 587, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("Enter_Sender_EmailAdress", "Enter_Sender_EmailAdress_Password");

                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            });
        }
    }
}
