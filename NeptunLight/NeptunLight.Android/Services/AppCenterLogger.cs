using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using NeptunLight.Services;

namespace NeptunLight.Droid.Services
{
    /// <summary>
    ///     Implements <see cref="ILogger"/> using the AppCenter SDK
    /// </summary>
    public class AppCenterLogger : ILogger
    {
        public void LogError(Exception ex, Dictionary<string, string> context = null)
        {
            Crashes.TrackError(ex, context);
        }
    }
}