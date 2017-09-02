using System;
using NeptunLight.Models;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class MessageViewModel : PageViewModel
    {
        private readonly ObservableAsPropertyHelper<string> _senderCode;

        private DateTime _date;
        private string _htmlContent;
        private string _sender;
        private string _subject;

        public MessageViewModel(Mail model)
        {
            Date = model.ReceivedTime;
            Subject = model.Subject;
            Sender = model.Sender;
            HtmlContent = model.Content;
            this.WhenAny(x => x.Sender, sender => string.IsNullOrEmpty(sender.Value) ? String.Empty : sender.Value.ToUpper().Substring(0, 1)).ToProperty(this, x => x.SenderCode, out _senderCode);
        }

        public DateTime Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }

        public string Subject
        {
            get => _subject;
            set => this.RaiseAndSetIfChanged(ref _subject, value);
        }

        public string HtmlContent
        {
            get => _htmlContent;
            set => this.RaiseAndSetIfChanged(ref _htmlContent, value);
        }

        public string Sender
        {
            get => _sender;
            set => this.RaiseAndSetIfChanged(ref _sender, value);
        }

        public string SenderCode => _senderCode.Value;
        public override string Title
        {
            get
            {
                if (string.IsNullOrEmpty(Subject))
                    return String.Empty;
                return Subject.Substring(0, 1).ToUpper() + Subject.Substring(1);
            }
        }
    }
}