using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using NeptunLight.Helpers;
using NeptunLight.Models;
using Newtonsoft.Json.Linq;

namespace NeptunLight.DataAccess
{
    public abstract class WebScraperClient
    {
        protected abstract HttpClient HttpClient { get; }

        public Uri BaseUri
        {
            get => HttpClient.BaseAddress;
            set
            {
                HttpClient.BaseAddress = value;
                HttpClient.DefaultRequestHeaders.Referrer = value;
            }
        }

        public async Task<string> GetRawAsnyc(string url, CancellationToken ct = default(CancellationToken))
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(url, ct))
            {
                if (!response.IsSuccessStatusCode)
                    throw await NetworkException.FromResponseAsync(response);
                return await ReadResponseAsync(response);
            }
        }

        public async Task<Stream> GetRawStreamAsnyc(string url, CancellationToken ct = default(CancellationToken))
        {
            HttpResponseMessage response = await HttpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
                throw await NetworkException.FromResponseAsync(response);

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<IDocument> GetDocumentAsnyc(string url, CancellationToken ct = default(CancellationToken))
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(url, ct))
            {
                if (!response.IsSuccessStatusCode)
                    throw await NetworkException.FromResponseAsync(response);

                HtmlParser parser = new HtmlParser();
                return await parser.ParseAsync(await ReadResponseAsync(response), ct);
            }
        }

        public async Task<IDocument> PostFormAsnyc(string url, IDocument form, ICollection<KeyValuePair<string, string>> overrides, bool keepOriginalHeaders = true, CancellationToken ct = default(CancellationToken))
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
                using (HttpResponseMessage response = await HttpClient.PostAsync(url, postContent, ct))
                {
                    if (!response.IsSuccessStatusCode)
                        throw await NetworkException.FromResponseAsync(response);

                    HtmlParser parser = new HtmlParser();
                    return await parser.ParseAsync(await ReadResponseAsync(response), ct);
                }
            }
        }

        public async Task<string> PostFormRawAsnyc(string url, IDocument form, IEnumerable<KeyValuePair<string, string>> overrides, CancellationToken ct = default(CancellationToken))
        {
            IEnumerable<KeyValuePair<string, string>> paramCollection = form.GetPostbackData()
                                                                            .Where(kvp => overrides.All(overrideKvp => overrideKvp.Key != kvp.Key))
                                                                            .Concat(overrides);
            using (HttpContent postContent = new FormUrlEncodedContent(paramCollection))
            {
                using (HttpResponseMessage response = await HttpClient.PostAsync(url, postContent, ct))
                {
                    if (!response.IsSuccessStatusCode)
                        throw await NetworkException.FromResponseAsync(response);

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
            catch (IOException)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<JObject> GetJsonObjectAsnyc(string url, CancellationToken ct = default(CancellationToken))
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(url, ct))
            {
                if (!response.IsSuccessStatusCode)
                    throw await NetworkException.FromResponseAsync(response);

                return JObject.Parse(await ReadResponseAsync(response));
            }
        }

        public async Task<JObject> PostJsonObjectAsnyc(string url, string json, CancellationToken ct = default(CancellationToken))
        {
            using (HttpResponseMessage response = await HttpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), ct))
            {
                if (!response.IsSuccessStatusCode)
                    throw await NetworkException.FromResponseAsync(response);

                return JObject.Parse(await ReadResponseAsync(response));
            }
        }
    }
}