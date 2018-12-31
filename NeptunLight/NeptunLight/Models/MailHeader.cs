using System;
using JetBrains.Annotations;

namespace NeptunLight.Models
{
    public class MailHeader
        : IEquatable<MailHeader>
    {

        #region Equality members
        public bool Equals([CanBeNull] MailHeader other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ReceivedTime.Equals(other.ReceivedTime) && string.Equals(Sender, other.Sender) && string.Equals(Subject, other.Subject);
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MailHeader) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ReceivedTime.GetHashCode();
                hashCode = (hashCode * 397) ^ Sender.GetHashCode();
                hashCode = (hashCode * 397) ^ Subject.GetHashCode();
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

        /// <summary>
        ///     An ID of this Mail. Guaranteed to be unique within a dataset, but may change or collide after a refresh.
        /// </summary>
        internal long TrId { set; get; }

        /// <summary>
        ///     True if this mail is newly fetched from the server. (Has not been fetched before)
        /// </summary>
        public bool IsNew { get; set; }
    }
}