using System.Threading.Tasks;
using NeptunLight.Models;

namespace NeptunLight.Services
{
    public interface IMailContentCache
    {
        Task StoreAsync(MailHeader key, Mail value);

        Task<bool> TryRetrieveAsync(MailHeader key, out Mail value);
    }
}