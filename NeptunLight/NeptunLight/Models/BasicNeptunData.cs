using ReactiveUI;

namespace NeptunLight.Models
{
    public class BasicNeptunData : ReactiveObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _neptunCode;

        public string NeptunCode
        {
            get => _neptunCode;
            set => this.RaiseAndSetIfChanged(ref _neptunCode, value);
        }

        private string _major;

        public string Major
        {
            get => _major;
            set => this.RaiseAndSetIfChanged(ref _major, value);
        }
    }
}