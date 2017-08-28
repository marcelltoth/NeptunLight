using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using NeptunLight.DataAccess;
using NeptunLight.Droid.Services;
using NeptunLight.Models;
using NeptunLight.Services;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid
{
    [Application]
    public class App : Application
    {
        protected App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public static MainActivity MainActivity { get; set; }

        public static readonly Lazy<IContainer> Container = new Lazy<IContainer>(() =>
        {
            ContainerBuilder builder = new ContainerBuilder();

            // built in classes
            builder.RegisterModule<AutofacModuleDefinition>();

            // service implementations
            builder.RegisterType<FileDataStorage>()
                   .As<IDataStorage>()
                   .SingleInstance();

            builder.Register(context => MainActivity)
                   .As<INavigator>();

            return builder.Build();
        });
    }
}