using System;

namespace NeptunLight.Models
{
    public class SyncException : Exception
    {
        public SyncException(string message) : base(message)
        {
        }

        public SyncException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}