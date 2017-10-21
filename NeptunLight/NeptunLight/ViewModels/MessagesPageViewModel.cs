using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class MessagesPageViewModel : PageViewModel
    {
        private IDataStorage DataStorage { get; }

        private readonly ObservableAsPropertyHelper<IReactiveDerivedList<MessageViewModel>> _messages;
        public IReactiveDerivedList<MessageViewModel> Messages => _messages.Value;

        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;

        public MessagesPageViewModel(IDataStorage data, INeptunInterface dataAccess, Func<Mail, MessageViewModel> messageVmFac, INavigator navigator)
        {
            DataStorage = data;
            this.WhenAnyValue(x => x.DataStorage.CurrentData.Messages).ObserveOn(RxApp.MainThreadScheduler).Select(msgList => msgList.CreateDerivedCollection(messageVmFac)).ToProperty(this, x => x.Messages, out _messages);


            OpenMessage = ReactiveCommand.Create<MessageViewModel, Unit>(vm =>
            {
                navigator.NavigateTo(vm);
                return Unit.Default;
            });

            RefreshMessages = ReactiveCommand.CreateFromTask(async () =>
            {
                IList<Mail> mail = await dataAccess.RefreshMessages().ToList();
                DataStorage.CurrentData.Messages.Clear();
                DataStorage.CurrentData.Messages.AddRange(mail);
                await DataStorage.SaveDataAsync();
            });

            RefreshMessages.IsExecuting.ToProperty(this, x => x.IsRefreshing, out _isRefreshing);
        }

        public ReactiveCommand<MessageViewModel, Unit> OpenMessage { get; }

        public ReactiveCommand RefreshMessages { get; }

        public override string Title { get; } = "Üzenetek";
    }
}