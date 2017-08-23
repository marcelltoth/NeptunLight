using System;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public class WebNeptunInterfaceFactory : INeptunInterfaceFactory
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Uri BaseUri { get; set; }
        public INeptunInterface Build()
        {
            return new WebNeptunInterface(Username, Password, BaseUri);
        }
    }
}