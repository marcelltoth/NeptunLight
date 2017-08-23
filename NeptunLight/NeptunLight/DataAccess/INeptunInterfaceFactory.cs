using System;

namespace NeptunLight.DataAccess
{
    public interface INeptunInterfaceFactory
    {
        string Username { get; set; }

        string Password { get; set; }

        Uri BaseUri { get; set; }

        INeptunInterface Build();
    }
}