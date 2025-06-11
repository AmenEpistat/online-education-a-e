using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace MathTrainer.Services
{
    public class EmailService
    {
        private readonly string _smtpHost = "smtp.mail.ru";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "luizaxakieva@mail.ru";
        private readonly string _smtpPass = "RPdPNgrkQwgP4nyBF8Y0";
        private readonly string _fromName = "Amen&Els";

        public async Task SendCertificateAsync(string toEmail, string userName, MemoryStream certificateStream)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _smtpUser));
            message.To.Add(new MailboxAddress(userName, toEmail));
            message.Subject = "Ваш сертификат с платформы Amen$Els";

            var body = new TextPart("plain")
            {
                Text = $"Здравствуйте, {userName}!\n\nВаш сертификат прикреплён к этому письму.\nУспехов в дальнейшем обучении!"
            };

            certificateStream.Position = 0;
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(certificateStream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "Certificate.pdf"
            };

            var multipart = new Multipart("mixed") { body, attachment };
            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
