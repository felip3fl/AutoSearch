using Microsoft.Extensions.Options;
using Search.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Search.Tools
{
    public record EmailMessage(
        string To,
        string Subject,
        string Body,
        bool IsHtml = false,
        IEnumerable<string>? Cc = null,
        IEnumerable<string>? Bcc = null,
        IEnumerable<Attachment>? Attachments = null
    );

    public class EmailService 
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            var client = CreateSmtpClient();
            var mailMessage = BuildMailMessage(new EmailMessage(toEmail, subject, body, false));

            await client.SendMailAsync(mailMessage);
        }

        private SmtpClient CreateSmtpClient() 
        {
            return new SmtpClient(_settings.Host)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = _settings.UseDefaultCredentials
            };
        }

        private MailMessage BuildMailMessage(EmailMessage message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.Username, _settings.FromName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml
            };

            mail.To.Add(message.To);

            foreach (var cc in message.Cc ?? [])
                mail.CC.Add(cc);

            foreach (var bcc in message.Bcc ?? [])
                mail.Bcc.Add(bcc);

            foreach (var attachment in message.Attachments ?? [])
                mail.Attachments.Add(attachment);

            return mail;
        }


    }
}
