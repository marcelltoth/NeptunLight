using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class LoginPageViewModel : PageViewModel
    {
        private IReadOnlyList<Institute> _avaialbleInstitutes;
        private string _loginCode = "asd";

        private string _password;

        public LoginPageViewModel(IInstituteDataProvider instituteDataProvider, INeptunInterfaceFactory neptunInterfaceFactory, Func<INeptunInterface, MenuPageViewModel> menuVmFactory)
        {
            AvaialbleInstitutes = instituteDataProvider.GetAvaialbleInstitutes().ToList();

            Login = ReactiveCommand.CreateFromTask(async ct => {
                                                       neptunInterfaceFactory.Username = LoginCode;
                                                       neptunInterfaceFactory.Password = Password;
                                                       INeptunInterface neptunInterface = neptunInterfaceFactory.Build();
                                                       await neptunInterface.LoginAsync();
                                                       return menuVmFactory(neptunInterface); },
                                                   this.WhenAny(
                                                       x => x.LoginCode,
                                                       x => x.Password,
                                                       (loginCode, password) => !string.IsNullOrEmpty(loginCode.Value) && !string.IsNullOrEmpty(password.Value)));
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

        public IReadOnlyList<Institute> AvaialbleInstitutes
        {
            get => _avaialbleInstitutes;
            set => this.RaiseAndSetIfChanged(ref _avaialbleInstitutes, value);
        }

        public ReactiveCommand<Unit,MenuPageViewModel> Login { get; }
    }
}