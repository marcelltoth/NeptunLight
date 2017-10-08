using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using NeptunLight.DataAccess;
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

        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;

        public MessagesPageViewModel(IDataStorage data, INeptunInterface dataAccess, IMailContentCache mailCache, Func<Mail, MessageViewModel> messageVmFac, INavigator navigator)
        {
            DataStorage = data;
            this.WhenAny(x => x.DataStorage.CurrentData.Messages, messages => messages.Value.Take(100).Select(messageVmFac).ToList()).ToProperty(this, x => x.Messages, out _messages);

            OpenMessage = ReactiveCommand.Create<MessageViewModel, Unit>(vm =>
            {
                navigator.NavigateTo(vm);
                return Unit.Default;
            });

            RefreshMessages = ReactiveCommand.CreateFromTask(async () =>
            {
                data.CurrentData.Messages = await dataAccess.RefreshMessagesAsnyc(mailCache);
                await data.SaveDataAsync();
            });

            RefreshMessages.IsExecuting.ToProperty(this, x => x.IsRefreshing, out _isRefreshing);
        }

        public ReactiveCommand<MessageViewModel, Unit> OpenMessage { get; }

        public ReactiveCommand RefreshMessages { get; }

        public override string Title { get; } = "Üzenetek";
    }
}