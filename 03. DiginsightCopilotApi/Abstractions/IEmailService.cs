namespace DiginsightCopilotApi.Abstractions;

public interface IEmailService
{
    Task SendMailAsync(string receiverMail, string mailTitle, string htmlBody);
    Task SendMailsAsync(
        IEnumerable<string> to,
        string mailTitle,
        string htmlBody,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null
    );
}
