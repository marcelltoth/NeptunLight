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
            yield return new Institute("BGE - Budapesti Gazdasági Egyetem", new Uri("https://neptun6.uni-bge.hu/hallgato/"));
            yield return new Institute("BME - Budapesti Műszaki és Gazdaságtudományi Egyetem", new Uri("https://frame.neptun.bme.hu/hallgatoi/"));
            yield return new Institute("DE - Debreceni Egyetem", new Uri("https://www-3.neptun.unideb.hu/hallgato/"));
            yield return new Institute("ELTE - Eötvös Loránd Tudományegyetem", new Uri("https://hallgato.neptun.elte.hu/"));
            yield return new Institute("OE - Óbudai Egyetem", new Uri("https://neptun.uni-obuda.hu/hallgato/"));
            yield return new Institute("PE - Pannon Egyetem", new Uri("https://neptun11.uni-pannon.hu/hallgato/"));
            yield return new Institute("PPKE - Pázmány Péter Katolikus Egyetem", new Uri("https://neptun2.ppke.hu/hallgato/"));
            yield return new Institute("SE - Semmelweis Egyetem", new Uri("https://neptunweb.semmelweis.hu/hallgato/"));
            yield return new Institute("SOE - Soproni Egyetem", new Uri("https://neptun3r.nyme.hu/hallgato/"));
            yield return new Institute("SZE - Széchenyi István Egyetem", new Uri("https://netw8.nnet.sze.hu/hallgato/"));
            yield return new Institute("SZIE - Szent István Egyetem", new Uri("https://web4.neptun.szie.hu/hallgato/"));
            yield return new Institute("SZTE - Szegedi Tudományegyetem", new Uri("https://web4.neptun.u-szeged.hu/hallgato/"));
        }
    }
}
