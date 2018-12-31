using System.Collections.Generic;
using NeptunLight.Models;

namespace NeptunLight.Services
{
    public interface IMailNotificationDispatcher
    {
        /// <summary>
        ///     Notifies the user that a single new Mail is received.
        /// </summary>
        /// <param name="mail">The received <see cref="Mail"/>.</param>
        void NotifyNewMail(Mail mail);

        /// <summary>
        ///     Sends a single notification to the user about multiple new messages received.
        /// </summary>
        /// <param name="newMails">The list of new <see cref="Mail"/>s received.</param>
        void NotifyMultipleNewMails(IEnumerable<Mail> newMails);
    }
}