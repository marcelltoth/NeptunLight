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
    public class LoginPageViewModel : PageViewModel
    {
        private IPrimitiveStorage PrimitigeStorage { get; }

        private const string DISCLAIMER_SETTING_KEY = "SHOULD_SHOW_DISCLAIMER";

        private IReadOnlyList<Institute> _avaialbleInstitutes;
        private string _loginCode;

        private string _loginError;

        private string _password;

        private Institute _selectedInstitute;

        public LoginPageViewModel(IInstituteDataProvider instituteDataProvider, INeptunInterface neptunInterface, INavigator navigator, IPrimitiveStorage storage)
        {
            PrimitigeStorage = storage;

            AvaialbleInstitutes = instituteDataProvider.GetAvaialbleInstitutes().ToList();

            Login = ReactiveCommand.CreateFromTask(async ct =>
                                                   {
                                                       LoginError = "";
                                                       neptunInterface.Username = LoginCode;
                                                       neptunInterface.Password = Password;
                                                       neptunInterface.BaseUri = SelectedInstitute.RootUrl;
                                                       await neptunInterface.LoginAsync();
                                                       navigator.NavigateTo<InitialSyncPageViewModel>(false);
                                                   },
                                                   this.WhenAny(
                                                       x => x.LoginCode,
                                                       x => x.Password,
                                                       x => x.SelectedInstitute,
                                                       (loginCode, password, inst) => !string.IsNullOrEmpty(loginCode.Value) && !string.IsNullOrEmpty(password.Value) && inst != null));

            Login.ThrownExceptions.Subscribe(e =>
            {
                LoginError = e is UnauthorizedAccessException ? "Hibás NEPTUN kód / jelszó." : "Hálózati hiba. Ellenőrizd az internetkapcsolatot és próbáld újra.";
            });
        }

        public string LoginCode
        {
            get => _loginCode;
            set => this.RaiseAndSetIfChanged(ref _loginCode, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public Institute SelectedInstitute
        {
            get => _selectedInstitute;
            set => this.RaiseAndSetIfChanged(ref _selectedInstitute, value);
        }

        public IReadOnlyList<Institute> AvaialbleInstitutes
        {
            get => _avaialbleInstitutes;
            set => this.RaiseAndSetIfChanged(ref _avaialbleInstitutes, value);
        }

        public ReactiveCommand<Unit,Unit> Login { get; }

        public string LoginError
        {
            get => _loginError;
            set => this.RaiseAndSetIfChanged(ref _loginError, value);
        }

        public override string Title { get; } = "Bejelentkezés";

        public bool ShouldShowDisclaimer
        {
            get => !PrimitigeStorage.ContainsKey(DISCLAIMER_SETTING_KEY) || (PrimitigeStorage.GetInt(DISCLAIMER_SETTING_KEY) == 1);
            set
            {
                this.RaisePropertyChanging();
                PrimitigeStorage.PutInt(DISCLAIMER_SETTING_KEY, value ? 1 : 0);
                this.RaisePropertyChanged();
            }
        }
    }
}