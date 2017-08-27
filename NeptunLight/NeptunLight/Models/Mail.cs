using System;

namespace NeptunLight.Models
{
    public class Mail
        : MailHeader
    {
        public Mail(DateTime receivedTime, string sender, string subject, string content)
            :base(receivedTime, sender, subject)
        {
            Content = content;
        }

        public string Content { get; }
    }
}