using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

using SendGrid;
using SendGrid.Helpers.Mail;

using UserManager.Api.Config;

namespace UserManager.Api.Impl
{
    public class SendGridEmailSender : IEmailSender
    {
        private SendGridSettings _settings;

        public SendGridEmailSender(IOptions<SendGridSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        private Task Execute(string subject, string message, string email)
        {
            var client = new SendGridClient(_settings.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_settings.SendGridFromAddress, _settings.SendGridFromAddress),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
