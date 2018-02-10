using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.Models;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class LoginPage : ReactiveFragment<LoginPageViewModel>
    {
        public ViewGroup LoginPanel { get; set; }
        public EditText LoginField { get; set; }
        public EditText PasswordField { get; set; }
        public Button LoginButton { get; set; }
        public Spinner InstituteSelector { get; set; }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View layout = inflater.Inflate(Resource.Layout.LoginPage, container, false);


            this.WireUpControls(layout);

            this.Bind(ViewModel, x => x.LoginCode, x => x.LoginField.Text);

            this.Bind(ViewModel, x => x.Password, x => x.PasswordField.Text);

            this.BindCommand(ViewModel, x => x.Login, x => x.LoginButton);
            LoginButton.Click += (sender, args) =>
            {
                Analytics.TrackEvent("Login attempt", new Dictionary<string, string>{ { "Institute", ViewModel.SelectedInstitute.Name}, {"User", ViewModel.LoginCode} });
            };

            this.WhenAnyObservable(x => x.ViewModel.Login).Subscribe(_ =>
            {
                Analytics.TrackEvent("Login successful", new Dictionary<string, string> { { "Institute", ViewModel.SelectedInstitute.Name }, { "User", ViewModel.LoginCode }});
            });

            this.WhenAnyValue(x => x.ViewModel.AvaialbleInstitutes).Subscribe(v => { InstituteSelector.Adapter = new InstituteAdapter(inflater, v.ToList()); });

            InstituteSelector.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedInstitute = ((InstituteAdapter) InstituteSelector.Adapter)[args.Position];
            };

            this.WhenAnyValue(x => x.ViewModel.LoginError).Skip(1).Where(err => !String.IsNullOrEmpty(err)).Subscribe(err =>
            {
                if(Activity != null)
                    Toast.MakeText(Activity, err, ToastLength.Short).Show();
            });

            if (ViewModel.ShouldShowDisclaimer)
            {
                new AlertDialog.Builder(Activity).SetTitle("Üdv")
                    .SetMessage("Örülök hogy letöltötted az appomat. Tudnod kell, hogy semmi közöm a hivatalos neptunhoz, én csak egy corvinusos diák vagyok akinek szúrta a szemét, hogy nincs egy értelmes mobilos nézet. "
                                + "Emiatt természetesen nincs lehetőségem mindent letesztelni, amit tudok megteszek, de előfordulhatnak bőven hibák, ezért persze felelősséget nem vállalhatok. "
                                + "Légyszi segíts nekem azzal hogy ezeket jelzed hogy tudjam javítani. (Google Play-en alul tudsz email-t írni) Köszi:)")
                                .SetPositiveButton("Értettem", (sender, args) => ViewModel.ShouldShowDisclaimer = false)
                                .Create().Show();
            }

            return layout;
        }

        public override void OnResume()
        {
            base.OnResume();

            bool resized = false;
            LoginPanel.ViewTreeObserver.GlobalLayout += (o, args) =>
            {
                if (resized)
                    return;
                resized = true;
                // only do this once, otherwise it gets recursive

                ViewGroup.LayoutParams layout = LoginPanel.LayoutParameters;
                int originalHeight = LoginPanel.Height;
                int originalWidth = LoginPanel.Width;
                double finalHeight = originalHeight;
                const double ratio = 1193 / 842d;
                if (originalWidth > originalHeight / ratio) // too wide
                    layout.Width = (int)(originalHeight / ratio);
                else // too tall
                {
                    layout.Height = (int)(originalWidth * ratio);
                    finalHeight = layout.Height;
                }
                LoginPanel.LayoutParameters = layout;
                LoginPanel.SetPadding(LoginPanel.PaddingLeft, (int)(finalHeight * 0.22), LoginPanel.PaddingRight, LoginPanel.PaddingBottom);
            };
        }

        private class InstituteAdapter : BaseAdapter<Institute>
        {
            private readonly LayoutInflater _inflater;

            public InstituteAdapter(LayoutInflater inflater, IList<Institute> elements)
            {
                _inflater = inflater;
                Elements = elements;
            }

            private IList<Institute> Elements { get; }

            public override int Count => Elements.Count;

            public override Institute this[int position] => Elements[position];

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, [CanBeNull] View convertView, [CanBeNull] ViewGroup parent)
            {
                View view = convertView ?? _inflater.Inflate(
                                Android.Resource.Layout.SimpleListItem1, parent, false);
                TextView content = view.FindViewById<TextView>(Android.Resource.Id.Text1);
                content.Text = Elements[position].Name;
                return view;
            }
        }
    }
}