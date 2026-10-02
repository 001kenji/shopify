using Microsoft.AspNetCore.Identity.UI.Services;

namespace Shop_Management_System.Services
{
    public interface IAppEmailSender : IEmailSender
    {
        Task SendTemplateAsync<TModel>(
            string toEmail,
            string? toName,
            string subject,
            string templateName,
            TModel model);
    }
}