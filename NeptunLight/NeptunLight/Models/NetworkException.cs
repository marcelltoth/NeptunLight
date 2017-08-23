using System;
using System.IO;

namespace NeptunLight.Models
{
    public class NetworkException : IOException
    {
        public NetworkException()
        {
        }

        public NetworkException(string message) : base(message)
        {
        }

        public NetworkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NetworkException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}