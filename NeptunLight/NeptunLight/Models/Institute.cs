using System;

namespace NeptunLight.Models
{
    public class Institute
    {
        public Institute(string name, Uri rootUrl)
        {
            Name = name;
            RootUrl = rootUrl;
        }

        public string Name { get; }

        public Uri RootUrl { get; }
    }
}