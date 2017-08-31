using System;
using System.Collections.Generic;
using System.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class MessagesPageViewModel : PageViewModel
    {
        private IDataStorage DataStorage { get; }

        private readonly ObservableAsPropertyHelper<IEnumerable<MessageViewModel>> _messages;
        public IEnumerable<MessageViewModel> Messages => _messages.Value;

        public MessagesPageViewModel(IDataStorage data, Func<Mail, MessageViewModel> messageVmFac)
        {
            DataStorage = data;
            this.WhenAny(x => x.DataStorage.CurrentData.Messages, messages => messages.Value.Select(messageVmFac)).ToProperty(this, x => x.Messages, out _messages);
        }
    }
}