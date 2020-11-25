using System;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace lmsextreg.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "][EmailSender][SendEmailAsync] = (Options.SendGridKey): " + Options.SendGridKey);
            return ExecuteOnPrem(subject, message, email);
        }

        public Task ExecuteOnPrem(string subject, string message, string toEmail)
        {
            Console.WriteLine("[EmailSender][ExecuteOnPrem]:" );

            var client = new SmtpClient
            {
                Host = "smtp.gsa.gov",
                Port = 25
            };
            //client.EnableSsl = true;
              
            // MailAddress fromAddress = new MailAddress("noreply-gsalearningacademy@gsa.gov", "GSA Learning Academy");
            MailAddress fromAddress = new MailAddress("gsalearningacademy+noreply@gsa.gov", "GSA Learning Academy");
            MailAddress toAddress   = new MailAddress(toEmail);
            
            MailMessage mailMessage     = new MailMessage(fromAddress, toAddress);
            mailMessage.IsBodyHtml      = true;
            mailMessage.Subject         = subject;
            mailMessage.Body            = message;
            //mailMessage.SubjectEncoding =  System.Text.Encoding.UTF8;
            //mailMessage.BodyEncoding    =  System.Text.Encoding.UTF8;

            return client.SendMailAsync(mailMessage);
        }
    }
}
