using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using NeptunLight.Models;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Pages
{
    public class LoginFragment : ReactiveFragment<LoginPageViewModel>
    {
        public EditText LoginField { get; set; }
        public EditText PasswordField { get; set; }
        public Button LoginButton { get; set; }
        public Spinner InstituteSelector { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View layout = inflater.Inflate(Resource.Layout.Login, container, false);


            this.WireUpControls(layout);
            this.Bind(ViewModel, x => x.LoginCode, x => x.LoginField.Text);
            this.Bind(ViewModel, x => x.Password, x => x.PasswordField.Text);
            this.BindCommand(ViewModel, x => x.Login, x => x.LoginButton);
            this.WhenAnyValue(x => x.ViewModel.AvaialbleInstitutes).Subscribe(v => { InstituteSelector.Adapter = new InstituteAdapter(inflater, v.ToList()); });
            this.WhenAnyValue(x => x.InstituteSelector.SelectedItemPosition).Select(x => ((InstituteAdapter) InstituteSelector.Adapter)[x]).BindTo(this, x => x.ViewModel.SelectedInstitute);

            return layout;
        }

        private class InstituteAdapter : BaseAdapter<Institute>
        {
            private readonly LayoutInflater _inflater;

            public InstituteAdapter(LayoutInflater inflater, IList<Institute> elements)
            {
                _inflater = inflater;
                Elements = elements;
            }

            public IList<Institute> Elements { get; }

            public override int Count => Elements.Count;

            public override Institute this[int position] => Elements[position];

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
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