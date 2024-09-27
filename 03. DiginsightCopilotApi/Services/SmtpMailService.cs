using DiginsightCopilotApi.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Mail;
using System.Text.RegularExpressions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using DiginsightCopilotApi.Abstractions;

namespace DiginsightCopilotApi.Services
{
    public class SmtpMailService: IEmailService
    {
        private readonly EmailConfig _mailConfig;
        private readonly ILogger<SmtpMailService> _log;
        private const string EMAILPATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public SmtpMailService(IOptions<EmailConfig> options, ILogger<SmtpMailService> log)
        {
            _mailConfig = options.Value;
            _log = log;
        }

        public async Task SendMailAsync(string receiverMail, string mailTitle, string htmlBody)
        {
            await SendEmailAsync([receiverMail], mailTitle, htmlBody);
        }

        public async Task SendMailsAsync(IEnumerable<string> to, string mailTitle, string htmlBody, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null)
        {
            await SendEmailAsync(to, mailTitle, htmlBody, cc, bcc);
        }

        private async Task SendEmailAsync(
            IEnumerable<string> receiversMail,
            string mailTitle,
            string htmlBody,
            IEnumerable<string>? cc = null,
            IEnumerable<string>? bcc = null
        )
        {
            IEnumerable<string> mails = receiversMail
                .Union(cc ?? Enumerable.Empty<string>())
                .Union(bcc ?? Enumerable.Empty<string>());
            foreach (var mail in mails)
            {
                if (!IsValidMail(mail))
                {
                    throw new ArgumentException($"{mail} is not a valid email");
                }
            }
            cc ??= new List<string>();
            bcc ??= new List<string>();

            var emailMessage = new MimeMessage();
            _log.LogInformation(
                "Sending mail to {email} from {senderMail}({senderName}) -> {smtp}:{port} {user}:{pass}",
                string.Join(";", receiversMail),
                _mailConfig.SenderMail,
                _mailConfig.SenderName,
                _mailConfig.Server,
                _mailConfig.Port,
                _mailConfig.SenderUsername,
                _mailConfig.Password.Substring(0, 3)
            );

            emailMessage.From.Add(new MailboxAddress(_mailConfig.SenderName, _mailConfig.SenderMail));
            foreach (var receiverMail in receiversMail)
            {
                emailMessage.To.Add(new MailboxAddress("", receiverMail));
            }

            foreach (var bccMail in bcc.Union(_mailConfig.BccMails).Distinct())
            {
                emailMessage.Bcc.Add(new MailboxAddress("", bccMail));
            }

            foreach (var ccMail in cc)
            {
                emailMessage.Cc.Add(new MailboxAddress("", ccMail));
            }

            emailMessage.Subject = mailTitle;
            emailMessage.Body = new TextPart("html") { Text = htmlBody };

            try
            {
                var client = new SmtpClient();

                if (_mailConfig.SecureConnection)
                {
                    if (_mailConfig.Server == "smtp-mail.outlook.com")
                    {
                        await client.ConnectAsync(
                            _mailConfig.Server,
                            _mailConfig.Port,
                            SecureSocketOptions.StartTls
                        );
                    }
                    else
                    {
                        await client.ConnectAsync(
                            _mailConfig.Server,
                            _mailConfig.Port,
                            SecureSocketOptions.SslOnConnect
                        );
                    }
                }
                else
                {
                    await client.ConnectAsync(
                        _mailConfig.Server,
                        _mailConfig.Port,
                        SecureSocketOptions.None
                    );
                }
                await client.AuthenticateAsync(
                    _mailConfig.SenderUsername,
                    "rlpx dldi ljta yfmi"//_mailConfig.Password
                );

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _log.LogError(
                    ex,
                    $"An error occurred sending mail to {string.Join(";", receiversMail)}"
                );
                throw;
            }
        }
        private bool IsValidMail(string email)
        {
            Regex emailRegex = new Regex(EMAILPATTERN);
            return emailRegex.IsMatch(email);
        }
    }
}
