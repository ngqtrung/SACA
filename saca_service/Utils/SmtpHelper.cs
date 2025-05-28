using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SACA_Infra.Const;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Infra.Config;

namespace SACA_Service.Utils
{
    public static class SmtpHelper
    {
        public static async Task SendMail(string toEmail, string subject, string emailContent, MailConfig EmailConfig)
        {
            var formAddress = new MailAddress(EmailConfig.Email, EmailConfig.Fullname);
            var toAddress = new MailAddress(toEmail);
            var mailMessage = new MailMessage(formAddress, toAddress);
            mailMessage.Subject = subject;
            var htmlView = AlternateView.CreateAlternateViewFromString(emailContent, null, MediaTypeNames.Text.Html);
            mailMessage.AlternateViews.Add(htmlView);
            var smtpClient = new SmtpClient(EmailConfig.SmtpClient_Host);
            smtpClient.Port = EmailConfig.SmtpClient_Port;
            smtpClient.Credentials = new NetworkCredential(EmailConfig.Email, EmailConfig.Password);
            smtpClient.EnableSsl = true;
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string BuildHtmlBody(string htmlFilePath, params KeyValuePair<string, string>[] dicts)
        {
            if (System.IO.File.Exists(htmlFilePath))
            {
                var html = System.IO.File.ReadAllText(htmlFilePath);
                foreach (var dict in dicts)
                {
                    html = html.Replace($"|{dict.Key}|", dict.Value);
                }
                return html;
            }
            throw new Exception("Body template is not exist");
        }
    }
}
