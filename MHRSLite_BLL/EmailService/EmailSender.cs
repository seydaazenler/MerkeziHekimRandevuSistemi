using MHRSLite_EL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MHRSLite_BLL.EmailService
{
    public class EmailSender : IEmailSender
    {
        //injection
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SenderMail => _configuration.GetSection("EmailOptions:SenderMail").Value;
        public string Password => _configuration.GetSection("EmailOptions:Password").Value;

        //smtp : simple mail transfer protocol - gmail
        public string Smtp => _configuration.GetSection("EmailOptions:Smtp").Value;
        public int SmtpPort => Convert.ToInt32(_configuration.GetSection("EmailOptions:SmtpPort").Value);
        public async Task SendAsync(EmailMessage message)
        {
            var mail = new MailMessage() 
            {
                From=new MailAddress(this.SenderMail)
            };
            //Contacts
            foreach (var item in message.Contacts)
            {
                mail.To.Add(item);
            }
            //cc
            if (message.CC!=null )
            {
                foreach (var item in message.CC)
                {
                    mail.CC.Add(new MailAddress(item));
                }
            }
            //bcc
            if (message.BCC != null)
            {
                foreach (var item in message.BCC)
                {
                    mail.Bcc.Add(new MailAddress(item));
                }
            }

            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.HeadersEncoding = Encoding.UTF8;

            var smtpClient = new SmtpClient(this.Smtp, this.SmtpPort)
            {
                EnableSsl = true,
                Credentials=new NetworkCredential(SenderMail,Password)

            };
            await smtpClient.SendMailAsync(mail);
        }
    }
}
