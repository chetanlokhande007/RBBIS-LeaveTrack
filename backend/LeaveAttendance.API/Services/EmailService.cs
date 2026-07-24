using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var host = _configuration["SmtpSettings:Host"];
                var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var username = _configuration["SmtpSettings:Username"];
                var password = _configuration["SmtpSettings:Password"];
                var from = _configuration["SmtpSettings:From"];

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false // Using plain text for OTP as per prompt
                };
                
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                // Depending on requirements, we can rethrow or swallow the exception
                throw new InvalidOperationException("Could not send email. Please try again later.", ex);
            }
        }
    }
}
