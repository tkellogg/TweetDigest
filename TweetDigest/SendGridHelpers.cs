using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using SendGridMail;
using TransportType = SendGridMail.TransportType;

namespace TweetDigest
{
    public class SendGridHelpers
    {
        public void SendEmail(string text)
        {
            var sendgrid = SendGridMail.SendGrid.GenerateInstance(
                from: new MailAddress("tkellogg@alteryx.com"),
                to: new[]
                    {
                        new MailAddress("timothy.kellogg@gmail.com")
                    },
                cc: new MailAddress[0], 
                bcc: new MailAddress[0],
                subject: "Your latest favorites",
                html: text,
                text: null,
                transport: TransportType.SMTP
            );

            var user = Config.SendGrid.User;
            var password = Config.SendGrid.Password;
            sendgrid.Mail(new NetworkCredential(user, password));
        }
    }
}