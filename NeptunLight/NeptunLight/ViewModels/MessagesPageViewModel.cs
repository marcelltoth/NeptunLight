using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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

        public MessagesPageViewModel(IDataStorage data, Func<Mail, MessageViewModel> messageVmFac, INavigator navigator)
        {
            DataStorage = data;
            this.WhenAny(x => x.DataStorage.CurrentData.Messages, messages => messages.Value.Take(100).Select(messageVmFac)).ToProperty(this, x => x.Messages, out _messages);

            OpenMessage = ReactiveCommand.Create<MessageViewModel, Unit>(vm =>
            {
                navigator.NavigateTo(vm);
                return Unit.Default;
            });
        }

        public ReactiveCommand<MessageViewModel, Unit> OpenMessage { get; }

        public override string Title { get; } = "Üzenetek";
    }
}