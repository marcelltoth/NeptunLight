using System.Threading.Tasks;
using JetBrains.Annotations;
using NeptunLight.Models;

namespace NeptunLight.Services
{
    public interface IMailContentCache
    {
        Task StoreAsync(MailHeader key, Mail value);

        [ItemCanBeNull]
        Task<Mail> TryRetrieveAsync(MailHeader key);
    }
}