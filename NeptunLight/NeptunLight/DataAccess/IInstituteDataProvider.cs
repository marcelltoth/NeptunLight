using System.Collections.Generic;
using System.Threading.Tasks;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public interface IInstituteDataProvider
    {
        IEnumerable<Institute> GetAvaialbleInstitutes();
    }
}