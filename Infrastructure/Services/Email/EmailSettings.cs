namespace Infrastructure.Services.EmailService;
public class EmailSettings
{
    public static string SectionName = "EmailConfiguration";
    public string? From { get; set; }
    public string? SmtpServer { get; set; }
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
