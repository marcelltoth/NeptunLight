using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public IReactiveDerivedList<MessageViewModel> Messages { get; }

        private readonly ObservableAsPropertyHelper<bool> _isRefreshing;
        public bool IsRefreshing => _isRefreshing.Value;

        public MessagesPageViewModel(IDataStorage data, INeptunInterface dataAccess, Func<Mail, MessageViewModel> messageVmFac, INavigator navigator)
        {
            DataStorage = data;
            Messages = DataStorage.CurrentData.Messages.CreateDerivedCollection(messageVmFac);

            OpenMessage = ReactiveCommand.Create<MessageViewModel, Unit>(vm =>
            {
                navigator.NavigateTo(vm);
                return Unit.Default;
            });

            RefreshMessages = ReactiveCommand.CreateFromTask(async () =>
            {
                DataStorage.CurrentData.Messages.Clear();
                await dataAccess.RefreshMessages().Do(mail => DataStorage.CurrentData.Messages.Add(mail)
                                                       , ex =>
                                                       {
                                                           Debug.WriteLine("Error" + ex.ToString());
                                                       },
                                                      async () =>
                                                      {
                                                          await DataStorage.SaveDataAsync();
                                                      });
            });

            RefreshMessages.IsExecuting.ToProperty(this, x => x.IsRefreshing, out _isRefreshing);
        }

        public ReactiveCommand<MessageViewModel, Unit> OpenMessage { get; }

        public ReactiveCommand RefreshMessages { get; }

        public override string Title { get; } = "Üzenetek";
    }
}