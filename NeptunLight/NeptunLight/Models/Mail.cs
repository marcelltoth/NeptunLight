using System;
using Newtonsoft.Json;

namespace NeptunLight.Models
{
    public class Mail
        : MailHeader
    {
        public Mail(MailHeader header, string content)
            : base(header.ReceivedTime, header.Sender, header.Subject)
        {
            Content = content;
        }

        [JsonConstructor]
        public Mail(DateTime receivedTime, string sender, string subject, string content)
            :base(receivedTime, sender, subject)
        {
            Content = content;
        }

        public string Content { get; }
    }
}