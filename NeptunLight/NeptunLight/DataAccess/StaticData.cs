using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public class StaticData : IInstituteDataProvider
    {
        public IEnumerable<Institute> GetAvaialbleInstitutes()
        {
            yield return new Institute("BCE - Budapesti Corvinus Egyetem", new Uri("https://neptun3r.web.uni-corvinus.hu/hallgatoi_2/"));
        }
    }
}
