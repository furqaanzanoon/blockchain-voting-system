using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Fallback console logging of OTP to ensure visibility even if SMTP is slow/failed
            var match = System.Text.RegularExpressions.Regex.Match(body, @"\b\d{6}\b");
            if (match.Success)
            {
                var otp = match.Value;
                Console.WriteLine("\n**************************************************");
                Console.WriteLine($"* [OTP CODE] {subject}");
                Console.WriteLine($"* TO:      {toEmail}");
                Console.WriteLine($"* CODE:    {otp}");
                Console.WriteLine("**************************************************\n");
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    var email = new MimeMessage();

                    email.From.Add(MailboxAddress.Parse(configuration["EmailSettings:Email"]!));
                    email.To.Add(MailboxAddress.Parse(toEmail));
                    email.Subject = subject;

                    email.Body = new TextPart("html")
                    {
                        Text = body
                    };

                    using var smtp = new SmtpClient();
                    smtp.Timeout = 5000; // 5 seconds timeout to prevent long hangs if SMTP is blocked/slow

                    await smtp.ConnectAsync(host: configuration["EmailSettings:Host"]!, port: int.Parse(configuration["EmailSettings:Port"]!), options: SecureSocketOptions.StartTls);

                    await smtp.AuthenticateAsync(userName: configuration["EmailSettings:Email"]!, password: configuration["EmailSettings:Password"]!);

                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EmailService WARNING] Failed to deliver email to {toEmail}: {ex.Message}");
                }
            });

            return Task.CompletedTask;
        }
    }
}