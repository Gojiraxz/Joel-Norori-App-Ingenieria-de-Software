using Joel_Norori_App_Ingenieria_de_Software.Models.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public class EmailService(IOptions<SmtpSettings> _SmtpSettings) : IEmailService
    {
        private readonly SmtpSettings _smtp = _SmtpSettings.Value;
        public void SendPasswordResetEmail(string toEmail, string body)
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.User, _smtp.Password),
                EnableSsl = true
            };
            var mail = new MailMessage
            {
                From= new MailAddress(_smtp.User,"Soporte de Autenticacion"),
                Subject="Token de confirmacion / Recuperacion",
                Body=body,
                IsBodyHtml=true
            };
            mail.To.Add(toEmail);
            client.Send(mail);
        }
    }
}
