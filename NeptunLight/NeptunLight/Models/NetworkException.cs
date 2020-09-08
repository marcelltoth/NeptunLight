using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NeptunLight.Models
{
    public class NetworkException : Exception
    {
        public HttpStatusCode ResponseCode { get; }

        public string Content { get; }

        public NetworkException(HttpStatusCode responseCode, [CanBeNull] string content)
        {
            ResponseCode = responseCode;
            Content = content;
        }

        public static async Task<NetworkException> FromResponseAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                return new NetworkException(response.StatusCode, content);
            }
            catch
            {
                // this is possible
                return new NetworkException(response.StatusCode, null);
            }
        }

        public NetworkException(string message, Exception innerException) : base(message, innerException)
        {
        }


    }
}