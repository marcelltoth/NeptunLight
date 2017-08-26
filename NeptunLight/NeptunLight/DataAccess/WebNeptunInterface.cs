using System;
using System.Net.Http;
using System.Threading.Tasks;
using NeptunLight.Models;
using Newtonsoft.Json.Linq;

namespace NeptunLight.DataAccess
{
    public class WebNeptunInterface : INeptunInterface
    {
        private WebScraperClient _client;

        public WebNeptunInterface(string username, string password, Uri baseUri)
        {
            Username = username;
            Password = password;
            BaseUri = baseUri;
        }

        public string Username { get; }

        public string Password { get; }

        public Uri BaseUri { get; }

        public async Task LoginAsync()
        {
            _client = new WebScraperClient
            {
                BaseUri = BaseUri
            };

            try
            {
                JObject r1 = await _client.PostJsonObjectAsnyc("Login.aspx/GetMaxTryNumber", "");
                JObject r2 = await _client.PostJsonObjectAsnyc("Login.aspx/CheckLoginEnable", $"{{user: \"{Username}\", pwd: \"{Password}\", UserLogin: null, GUID: null, captcha: \"\"}}");
                JObject loginResult = JObject.Parse(r2.Value<string>("d"));
                if (!string.Equals(loginResult.Value<string>("success"), "True", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException();
                }
                JObject r3 = await _client.PostJsonObjectAsnyc("Login.aspx/SavePopupState", "{state: \"hidden\", PopupID: \"upLoginWait_popupLoginWait\"}");
            }
            catch (Exception exc) when (!(exc is UnauthorizedAccessException))
            {
                throw new NetworkException("Error loading neptun", exc);
            }
        }

        public Task<NeptunData> RefreshDataAsnyc(IProgress<string> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}