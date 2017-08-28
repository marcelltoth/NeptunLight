using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Autofac;
using NeptunLight.Droid.Pages;
using NeptunLight.Services;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid
{
	[Activity (Label = "NeptunLight.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, INavigator
	{
	    private static readonly Dictionary<Type, Type> VmToFragment;

	    static MainActivity()
	    {
	        string targetNamespace = $"{nameof(NeptunLight)}.{nameof(Droid)}.{nameof(Pages)}";
	        VmToFragment = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => ti.Namespace == targetNamespace && typeof(ReactiveFragment).IsAssignableFrom(ti))
	                .ToDictionary(ti => ti.BaseType.GenericTypeArguments[0], ti => ti.AsType());
	    }


        protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

		    App.MainActivity = this;

			SetContentView(Resource.Layout.Main);
            
		    NavigateTo<MenuPageViewModel>(false);
		}

	    public void NavigateTo<T>(bool addToStack = true) where T : PageViewModel
	    {
            NavigateTo(typeof(T), addToStack);
        }

	    public void NavigateTo(Type destinationVm, bool addToStack = true)
	    {
	        NavigateTo((PageViewModel)App.Container.Value.Resolve(destinationVm));
	    }

	    public void NavigateTo(PageViewModel destinationVm, bool addToStack = true)
	    {
	        Fragment fragment = (Fragment)Activator.CreateInstance(VmToFragment[destinationVm.GetType()]);
	        ((IViewFor) fragment).ViewModel = destinationVm;

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
	        transaction.Replace(Resource.Id.fragmentHolder, fragment);
	        if (addToStack)
	        {
	            transaction.AddToBackStack(null);
	        }
	        transaction.Commit();
        }

	    public void NavigateUp<T>() where T : PageViewModel
	    {
            if(FragmentManager.BackStackEntryCount > 0)
	            FragmentManager.PopBackStack();
        }
    }
}


