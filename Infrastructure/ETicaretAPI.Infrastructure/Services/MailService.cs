using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new MailMessage();
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;
            foreach(var to in tos)
            {
                mail.To.Add(to);
            }
            mail.From = new MailAddress(_configuration["Mail:Email"],"miniticaret", Encoding.UTF8);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_configuration["Mail:Email"], _configuration["Mail:Password"]);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.Append("Merhaba <br> Eğer yeni şifre talebinde bulunduysanız aşağıdaki bağlantıdan şifrenizi sıfırlayabilirsiniz" +
                "<br><strong><a target= \"_blank\" href =\"");
            mail.Append(_configuration["AngularClientUrl"]);
            mail.Append("/update-password/");
            mail.Append(userId);
            mail.Append("/");
            mail.Append(resetToken);
            mail.Append("\"> Yeni şifre talebi için tıklayınız </a></strong> <br><br>" +
                " <span style =\"font-size:12px;\">Not: Eğer bu linki talep etmediyseniz, maili ciddiye almayınız</span><br>");

            await SendMailAsync(to, "Şifre sıfırlama talebi", mail.ToString());

        }
    }
}
