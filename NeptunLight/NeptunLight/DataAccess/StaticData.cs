using System;
using System.Collections.Generic;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public class StaticData : IInstituteDataProvider
    {
        public IEnumerable<Institute> GetAvaialbleInstitutes()
        {
            yield return new Institute("BCE - Budapesti Corvinus Egyetem", new Uri("https://neptun3r.web.uni-corvinus.hu/hallgatoi_2/"));
            yield return new Institute("BME - Budapesti Műszaki és Gazdaságtudományi Egyetem", new Uri("https://frame.neptun.bme.hu/hallgatoi/"));
            yield return new Institute("PE - Pannon Egyetem", new Uri("https://neptun11.uni-pannon.hu/hallgato/"));
            yield return new Institute("SE - Semmelweis Egyetem", new Uri("https://neptunweb.semmelweis.hu/hallgato/"));
            yield return new Institute("SZE - Széchenyi István Egyetem", new Uri("https://netw8.nnet.sze.hu/hallgato/"));
        }
    }
}
