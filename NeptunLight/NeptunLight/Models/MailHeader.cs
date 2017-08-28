using System;

namespace NeptunLight.Models
{
    public class MailHeader
        : IEquatable<MailHeader>
    {

        #region Equality members
        public bool Equals(MailHeader other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ReceivedTime.Equals(other.ReceivedTime) && string.Equals(Sender, other.Sender) && string.Equals(Subject, other.Subject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MailHeader) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ReceivedTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (Sender != null ? Sender.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                return hashCode;
            }
        }

#endregion

        public MailHeader(DateTime receivedTime, string sender, string subject)
        {
            ReceivedTime = receivedTime;
            Sender = sender;
            Subject = subject;
        }

        public DateTime ReceivedTime { get; }
        public string Sender { get; }
        public string Subject { get; }

        internal long TrId { set; get; }
    }
}