using System.Collections.Generic;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public interface IInstituteDataProvider
    {
        IEnumerable<Institute> GetAvaialbleInstitutes();
    }
}