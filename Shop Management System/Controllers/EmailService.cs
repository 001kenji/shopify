using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Text.RegularExpressions;

namespace Shop_Management_System.Services
{
    public class EmailService : IEmailSender, IAppEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IViewRenderService _viewRenderer;

        public EmailService(IConfiguration configuration, IViewRenderService viewRenderer)
        {
            _configuration = configuration;
            _viewRenderer = viewRenderer;
        }

        // ---- IEmailSender contract (raw HTML) ----
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendAsync(email, subject, htmlMessage, toName: "");
        }

        // ---- Template-driven overload ----
        public async Task SendTemplateAsync<TModel>(
            string toEmail,
            string? toName,
            string subject,
            string templateName,
            TModel model)
        {
            var html = await _viewRenderer.RenderToStringAsync($"~/Views/EmailTemplates/{templateName}.cshtml", model);
            await SendAsync(toEmail, subject, html, toName ?? "");
        }

        // ---- Core MailKit sender ----
        private async Task SendAsync(string toEmail, string subject, string htmlMessage, string toName)
        {
            var fromEmail = _configuration["Smtp:Username"]
                ?? throw new InvalidOperationException("Smtp:Username is missing.");
            var senderName = _configuration["Smtp:SenderName"] ?? "Shopify Management";

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(senderName, fromEmail));
            emailMessage.To.Add(new MailboxAddress(toName, toEmail));
            emailMessage.Subject = subject;

            // Plain text fallback for clients that don't render HTML
            var plainText = Regex.Replace(htmlMessage, "<[^>]*>", string.Empty);
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage,
                TextBody = plainText
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Smtp:Host"],
                int.Parse(_configuration["Smtp:Port"]!),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(fromEmail, _configuration["Smtp:Password"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}