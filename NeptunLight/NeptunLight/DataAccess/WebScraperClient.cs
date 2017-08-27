using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using NeptunLight.Helpers;
using NeptunLight.Models;
using Newtonsoft.Json.Linq;

namespace NeptunLight.DataAccess
{
    public class WebScraperClient
    {
        public WebScraperClient()
        {
            HttpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });

            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0");
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html", 0.9));
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml", 0.9));
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("hu-HU", 0.5));
            HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            HttpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        private HttpClient HttpClient { get; }

        public Uri BaseUri
        {
            get => HttpClient.BaseAddress;
            set
            {
                HttpClient.BaseAddress = value;
                HttpClient.DefaultRequestHeaders.Referrer = value;
            }
        }

        public async Task<string> GetRawAsnyc(string url)
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                    throw new NetworkException();

                return await ReadResponseAsync(response);
            }
        }

        public async Task<Stream> GetRawStreamAsnyc(string url)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new NetworkException();

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<IDocument> GetDocumentAsnyc(string url)
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                    throw new NetworkException();

                HtmlParser parser = new HtmlParser();
                return await parser.ParseAsync(await ReadResponseAsync(response));
            }
        }

        public async Task<IDocument> PostFormAsnyc(string url, IDocument form, ICollection<KeyValuePair<string, string>> overrides, bool keepOriginalHeaders = true)
        {
            IEnumerable<KeyValuePair<string, string>> paramCollection = overrides;
            if (keepOriginalHeaders)
                paramCollection = form.GetPostbackData()
                                      .Where(kvp => overrides.All(overrideKvp => overrideKvp.Key != kvp.Key))
                                      .Concat(overrides);
            using (HttpContent postContent = new FormUrlEncodedContent(paramCollection))
            {
                postContent.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                postContent.Headers.TryAddWithoutValidation("X-MicrosoftAjax", "Delta=true");
                postContent.Headers.TryAddWithoutValidation("Accept", "*/*");
                using (HttpResponseMessage response = await HttpClient.PostAsync(url, postContent))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new NetworkException();

                    HtmlParser parser = new HtmlParser();
                    return await parser.ParseAsync(await ReadResponseAsync(response));
                }
            }
        }

        public async Task<string> PostFormRawAsnyc(string url, IDocument form, IEnumerable<KeyValuePair<string, string>> overrides)
        {
            IEnumerable<KeyValuePair<string, string>> paramCollection = form.GetPostbackData()
                                                                            .Where(kvp => overrides.All(overrideKvp => overrideKvp.Key != kvp.Key))
                                                                            .Concat(overrides);
            using (HttpContent postContent = new FormUrlEncodedContent(paramCollection))
            {
                using (HttpResponseMessage response = await HttpClient.PostAsync(url, postContent))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new NetworkException();

                    return await ReadResponseAsync(response);
                }
            }
        }

        private static async Task<string> ReadResponseAsync(HttpResponseMessage response)
        {
            Stream stream = await response.Content.ReadAsStreamAsync();
            try
            {
                using (GZipStream decompress = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(decompress))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (InvalidDataException)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<JObject> GetJsonObjectAsnyc(string url)
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                    throw new NetworkException();

                return JObject.Parse(await ReadResponseAsync(response));
            }
        }

        public async Task<JObject> PostJsonObjectAsnyc(string url, string json)
        {
            using (HttpResponseMessage response = await HttpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")))
            {
                if (!response.IsSuccessStatusCode)
                    throw new NetworkException();

                return JObject.Parse(await ReadResponseAsync(response));
            }
        }
    }
}