using System;
using System.Collections.Generic;

namespace NeptunLight.Services
{
    public interface ILogger
    {
        void LogError(Exception ex, Dictionary<string, string> context = null);
    }
}