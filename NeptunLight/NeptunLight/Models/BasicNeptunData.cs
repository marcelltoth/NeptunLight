namespace NeptunLight.Models
{
    public class BasicNeptunData
    {
        public BasicNeptunData(string name, string neptunCode, string major)
        {
            Name = name;
            NeptunCode = neptunCode;
            Major = major;
        }

        public string Name { get; }

        public string NeptunCode { get; }

        public string Major { get; }
    }
}