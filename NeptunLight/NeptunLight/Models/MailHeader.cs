using System;

namespace NeptunLight.Models
{
    public class MailHeader
    {
        public MailHeader(DateTime receivedTime, string sender, string subject)
        {
            ReceivedTime = receivedTime;
            Sender = sender;
            Subject = subject;
        }

        public DateTime ReceivedTime { get; }
        public string Sender { get; }
        public string Subject { get; }
    }
}