using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using NeptunLight.DataAccess;
using NeptunLight.Services;
using NeptunLight.ViewModels;
using ReactiveUI;

using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using ReactiveFragment = ReactiveUI.AndroidSupport.ReactiveFragment;

namespace NeptunLight.Droid
{
	[Activity (Label = "Neptun Lite", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
	public class MainActivity : AppCompatActivity, INavigator
	{
	    private readonly Dictionary<Type, PageViewModel> _pageViewModelCache = new Dictionary<Type, PageViewModel>();
	    private static readonly Dictionary<Type, Type> _vmToFragment;

	    private View _fragmentHolder;

	    static MainActivity()
	    {
	        string targetNamespace = $"{nameof(NeptunLight)}.{nameof(Droid)}.{nameof(Views)}";
	        _vmToFragment = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => ti.Namespace == targetNamespace && typeof(ReactiveFragment).IsAssignableFrom(ti))
	                                // ReSharper disable once PossibleNullReferenceException
	                .ToDictionary(ti => ti.BaseType.GenericTypeArguments[0], ti => ti.AsType());
	    }

	    protected override void OnSaveInstanceState([CanBeNull] Bundle outState)
	    {
            // workaround
	    }

	    protected override void OnCreate ([CanBeNull] Bundle bundle)
		{
            base.OnCreate(bundle);

		    AppCenter.Start(Secrets.AppCenterApiKey,
		                    typeof(Analytics), typeof(Crashes), typeof(Push));

            PreferenceManager.SetDefaultValues(this, Resource.Layout.SettingsPage, false);


		    App.MainActivity = this;

			SetContentView(Resource.Layout.Main);

		    _fragmentHolder = FindViewById(Resource.Id.fragmentHolder);
		    NavigateTo<MenuPageViewModel>(false);


            Observable.FromEventPattern(del => SupportFragmentManager.BackStackChanged += del, del => SupportFragmentManager.BackStackChanged -= del, RxApp.MainThreadScheduler)
	            .Subscribe(args =>
		    {
		        Fragment activeFragment = SupportFragmentManager.FindFragmentByTag("ACTIVE");
		        if (activeFragment != null)
		        {
		            Title = ((PageViewModel) ((IViewFor) activeFragment).ViewModel).Title;
		            ConfigureActionBar(activeFragment);
		        }
                else
		            Finish();
		    });

		    StartService(new Intent(BaseContext, typeof(RefreshService)));

            // Log institute statistics
		    var neptunInterface = App.Container.Resolve<INeptunInterface>();
		    var instituteProvider = App.Container.Resolve<IInstituteDataProvider>();
            var institute = instituteProvider.GetAvaialbleInstitutes().FirstOrDefault(inst => inst.RootUrl == neptunInterface.BaseUri)?.Name ?? neptunInterface.BaseUri?.ToString() ?? String.Empty;
            Analytics.TrackEvent("Application started", new Dictionary<string, string>()
            {
                { "Institute", institute }
            });
		}


	    private void ConfigureActionBar(Fragment activeFragment)
	    {
	        if (activeFragment is IActionBarProvider)
	            SupportActionBar.Show();
	        else
	            SupportActionBar.Hide();

            SupportActionBar.SetDisplayHomeAsUpEnabled(SupportFragmentManager.BackStackEntryCount > 0);

            // clean up after previous fragment
            SupportActionBar.RemoveAllTabs();
	    }

	    public override bool OnSupportNavigateUp()
	    {
	        if (SupportFragmentManager.BackStackEntryCount > 0)
	        {
		        SupportFragmentManager.PopBackStack();
	            return false;
	        }
	        else
	        {
	            Finish();
	            return true;
	        }
        }

	    public void NavigateTo<T>(bool addToStack = true) where T : PageViewModel
	    {
            NavigateTo(typeof(T), addToStack);
        }

	    public void NavigateTo(Type destinationVm, bool addToStack = true)
	    {
		    // Only instanciate one root view model once.
            if(!_pageViewModelCache.TryGetValue(destinationVm, out PageViewModel vm))
            {
                vm = (PageViewModel)App.Container.Resolve(destinationVm);
                _pageViewModelCache[destinationVm] = vm;
            }

            NavigateTo(vm, addToStack);
	    }

	    public void NavigateTo(PageViewModel destinationVm, bool addToStack = true)
	    {
	        _fragmentHolder.RequestFocus();

            Fragment fragment = (Fragment)Activator.CreateInstance(_vmToFragment[destinationVm.GetType()]);
	        ((IViewFor) fragment).ViewModel = destinationVm;
	        fragment.RetainInstance = true;

            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
	        transaction.Replace(Resource.Id.fragmentHolder, fragment, "ACTIVE");
	        if (addToStack)
	        {
	            transaction.AddToBackStack(null);
	        }
	        transaction.Commit();
	        Title = destinationVm.Title;
	        ConfigureActionBar(fragment);
	    }

	    public void NavigateUp()
	    {
            if(SupportFragmentManager.BackStackEntryCount > 0)
	            SupportFragmentManager.PopBackStack();
        }
    }
}


