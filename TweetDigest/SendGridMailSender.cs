using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ActionMailer.Net;
using SendGridMail;
using TransportType = SendGridMail.TransportType;

namespace TweetDigest
{
    public class SendGridMailSender : IMailSender
    {
        private readonly NetworkCredential credential;

        public SendGridMailSender(NetworkCredential credential)
        {
            this.credential = credential;
        }

        public void Send(MailMessage mail)
        {
            var body = mail.AlternateViews[0].ContentStream.ReadToEnd();
            var instance = SendGrid.GenerateInstance(mail.From, mail.To.ToArray(), mail.CC.ToArray(), mail.Bcc.ToArray(),
                                                     mail.Subject, body, null, TransportType.REST);
            instance.Mail(credential);
        }

        public void SendAsync(MailMessage mail, Action<MailMessage> callback)
        {
            Task.Run(() =>
                {
                    Send(mail);
                    callback(mail);
                });
        }

        public void Dispose()
        {
        }
    }
}