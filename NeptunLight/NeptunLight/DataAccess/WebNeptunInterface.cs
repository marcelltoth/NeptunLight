using System;
using System.Threading.Tasks;

namespace NeptunLight.DataAccess
{
    public class WebNeptunInterface : INeptunInterface
    {
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
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}