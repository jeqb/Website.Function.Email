﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace Website.Function.Email.EmailClient
{
    public class SmtpEmailClient : IEmailClient
    {
        private readonly string _smtpServer;

        private readonly int _smtpPort;
        // email address
        private readonly string _userName;

        private readonly string _password;

        public SmtpEmailClient(string smtpServer, int smtpPort, string userName, string password)
        {
            _smtpServer = smtpServer;

            _smtpPort = smtpPort;

            _userName = userName;

            _password = password;
        }

        /// <summary>
        /// true = email successfully sent
        /// false = email failed
        /// </summary>
        /// <param name="recipientAddress"></param>
        /// <param name="subject"></param>
        /// <param name="htmlBody"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            // build the message
            MimeMessage email = new();
            email.From.Add(MailboxAddress.Parse(_userName));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };


            // TODO: deal with concurrent connections limit with outlook.
            // might consider single thread.
            // see: https://aka.ms/concurrent_sending
            using (SmtpClient smtp = new())
            {
                await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_userName, _password);

                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);
            }
        }
    }
}
