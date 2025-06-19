using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SpeakCoreApp.Server.Common.Interfaces;
using SpeakCoreApp.Server.DTOs;

namespace SpeakCoreApp.Server.Common.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpSetting? _smtpSettings = new SmtpSetting();
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSetting>();
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string firstName, string registrationId)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password),
                    EnableSsl = true,
                };

                var subject = "Registration Confirmation - Welcome!";
                var body = GetEmailTemplate(firstName, registrationId);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Email, "Registration System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log error - in production, use proper logging
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        private string GetEmailTemplate(string firstName, string registrationId)
        {
            return $@"
        <html>
        <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; color: white; border-radius: 10px 10px 0 0;'>
                <h1 style='margin: 0; font-size: 28px;'>Welcome to SpeakCore!</h1>
            </div>
            <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
                <h2 style='color: #333; margin-top: 0;'>Hello {firstName}!</h2>
                <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                    Thank you for registering with us! Your registration has been successfully completed.
                </p>
                <div style='background: white; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #667eea;'>
                    <p style='margin: 0; color: #333;'><strong>Registration ID:</strong> {registrationId}</p>
                </div>
                <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                    Please keep this registration ID for your records. You can now log in to your account and start exploring our platform.
                </p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='http://localhost:4200' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 12px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; display: inline-block;'>
                        Go to Login
                    </a>
                </div>
                <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
                <p style='color: #999; font-size: 14px; text-align: center; margin: 0;'>
                    This is an automated message. Please do not reply to this email.
                </p>
            </div>
        </body>
        </html>";
        }
    }
}
