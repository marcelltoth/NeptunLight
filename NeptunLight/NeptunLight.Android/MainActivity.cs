using System;
using System.Collections.Generic;
using System.Linq;
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
using NeptunLight.Services;
using NeptunLight.ViewModels;
using ReactiveUI;
using Fragment = Android.App.Fragment;
using FragmentTransaction = Android.App.FragmentTransaction;

namespace NeptunLight.Droid
{
	[Activity (Label = "Neptun Lite", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
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

		    AppCenter.Start("b8c21b5f-b87f-4ac2-8645-019102f6d3d7",
		                    typeof(Analytics), typeof(Crashes));

            PreferenceManager.SetDefaultValues(this, Resource.Layout.SettingsPage, false);


		    App.MainActivity = this;

			SetContentView(Resource.Layout.Main);

		    _fragmentHolder = FindViewById(Resource.Id.fragmentHolder);
		    NavigateTo<MenuPageViewModel>(false);


            FragmentManager.Events().BackStackChanged.Subscribe(args =>
		    {
		        Fragment activeFragment = FragmentManager.FindFragmentByTag("ACTIVE");
		        if (activeFragment != null)
		        {
		            Title = ((PageViewModel) ((IViewFor) activeFragment).ViewModel).Title;
		            ConfigureActionBar(activeFragment);
		        }
                else
		            Finish();
		    });

		    StartService(new Intent(BaseContext, typeof(RefreshService)));
		}

	    protected override void OnStart()
	    {
	        base.OnStart();
        }

	    private void ConfigureActionBar(Fragment activeFragment)
	    {
	        if (activeFragment is IActionBarProvider)
	            SupportActionBar.Show();
	        else
	            SupportActionBar.Hide();

            SupportActionBar.SetDisplayHomeAsUpEnabled(FragmentManager.BackStackEntryCount > 0);

            // clean up after previous fragment
            SupportActionBar.RemoveAllTabs();
	    }

	    public override bool OnSupportNavigateUp()
	    {
	        if (FragmentManager.BackStackEntryCount > 0)
	        {
	            FragmentManager.PopBackStack();
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

	        PageViewModel vm;
            // Only instanciate one root view model once.
            if(!_pageViewModelCache.TryGetValue(destinationVm, out vm))
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

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
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
            if(FragmentManager.BackStackEntryCount > 0)
	            FragmentManager.PopBackStack();
        }
    }
}


