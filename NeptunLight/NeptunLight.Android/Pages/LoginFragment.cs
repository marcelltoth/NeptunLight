using Android.OS;
using Android.Views;
using Android.Widget;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Pages
{
    public class LoginFragment : ReactiveFragment<LoginPageViewModel>
    {

        public EditText LoginField { get; set; }
        public EditText PasswordField { get; set; }

        public Button LoginButton { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View layout = inflater.Inflate(Resource.Layout.Login, container, false);


            this.WireUpControls(layout);
            this.Bind(ViewModel, x => x.LoginCode, x => x.LoginField.Text);
            this.Bind(ViewModel, x => x.Password, x => x.PasswordField.Text);
            this.BindCommand(ViewModel, x => x.Login, x => x.LoginButton);


            return layout;
        }
    }
}