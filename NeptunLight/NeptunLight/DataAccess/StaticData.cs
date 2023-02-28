using System;
using System.Collections.Generic;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public class StaticData : IInstituteDataProvider
    {
        public IEnumerable<Institute> GetAvaialbleInstitutes()
        {
            yield return new Institute("BCE - Budapesti Corvinus Egyetem", new Uri("https://neptun3r.web.uni-corvinus.hu/Hallgatoi/"));
            yield return new Institute("BGE - Budapesti Gazdasági Egyetem", new Uri("https://neptun12.uni-bge.hu/hallgato/"));
            yield return new Institute("BME - Budapesti Műszaki és Gazdaságtudományi Egyetem", new Uri("https://frame.neptun.bme.hu/hallgatoi/"));
            yield return new Institute("DE - Debreceni Egyetem", new Uri("https://www-3.neptun.unideb.hu/hallgato/"));
            yield return new Institute("DUE - Dunaújvárosi Egyetem", new Uri("https://nappw.dfad.duf.hu/hallgato/"));
            yield return new Institute("EDUTUS - Edutus Egyetem", new Uri("https://neptun.edutus.hu/hallgato/"));
            yield return new Institute("EKE - Eszterházy Károly Egyetem", new Uri("https://neptunh.uni-eszterhazy.hu/Hallgato/"));
            yield return new Institute("ELTE - Eötvös Loránd Tudományegyetem", new Uri("https://hallgato.neptun.elte.hu/"));
            yield return new Institute("GDF - Gábor Dénes Főiskola", new Uri("https://neptun.gdf.hu/hallgato/"));
            yield return new Institute("GFF - Gál Ferenc Főiskola", new Uri("https://host.sdakft.hu/gffhw/"));
            yield return new Institute("KE - Kaposvári Egyetem", new Uri("https://neptun.ke.hu/hallgato/"));
            yield return new Institute("KRE - Károli Gáspár Református Egyetem", new Uri("https://neptun.kre.hu/hallgato/"));
            yield return new Institute("ME - Miskolci Egyetem", new Uri("https://neptun32.uni-miskolc.hu/hallgato/"));
            yield return new Institute("METU - Budapesti Metropolitan Egyetem", new Uri("https://neptunweb1.metropolitan.hu/hallgato/"));
            yield return new Institute("MILTON - Milton Friedman Egyetem", new Uri("https://neptun.uni-milton.hu/hallgato/"));
            yield return new Institute("NJE - Neumann János Egyetem", new Uri("https://neptun-web2.kefo.hu/hallgato/"));
            yield return new Institute("NKE - Nemzeti Közszolgálati Egyetem", new Uri("https://neptunweb.uni-nke.hu/hallgato/"));
            yield return new Institute("NYE - Nyíregyházi Egyetem", new Uri("https://neptunwebv1.nyf.hu/hallgatoi/"));
            yield return new Institute("OE - Óbudai Egyetem", new Uri("https://neptun.uni-obuda.hu/hallgato/"));
            yield return new Institute("PE - Pannon Egyetem", new Uri("https://neptun11.uni-pannon.hu/hallgato/"));
            yield return new Institute("PPKE - Pázmány Péter Katolikus Egyetem", new Uri("https://neptun2.ppke.hu/hallgato/"));
            yield return new Institute("PTE - Pécsi Tudományegyetem ", new Uri("https://neptun-web2.tr.pte.hu/hallgato/"));
            yield return new Institute("SE - Semmelweis Egyetem", new Uri("https://neptunweb.semmelweis.hu/hallgato/"));
            yield return new Institute("SOE - Soproni Egyetem", new Uri("https://neptun3r.nyme.hu/hallgato/"));
            yield return new Institute("SZE - Széchenyi István Egyetem", new Uri("https://netw8.nnet.sze.hu/hallgato/"));
            yield return new Institute("SZIE - Szent István Egyetem", new Uri("https://web4.neptun.szie.hu/hallgato/"));
            yield return new Institute("SZTE - Szegedi Tudományegyetem", new Uri("https://web4.neptun.u-szeged.hu/hallgato/"));
            yield return new Institute("TE - Testnevelési Egyetem", new Uri("https://neptun.tf.hu/hallgato/"));
        }
    }
}
