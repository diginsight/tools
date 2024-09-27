using System.ComponentModel.DataAnnotations;

namespace DiginsightCopilotApi.Configuration;

public class EmailConfig
{
    [Required]
    public string Server { get; set; }

    [Required]
    public int Port { get; set; }

    [Required]
    public string SenderName { get; set; }

    [Required]
    public string SenderMail { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string SenderUsername { get; set; }

    public bool SecureConnection { get; set; }

    public string[] BccMails { get; set; } = [];
}
