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

            // viewmodels
            builder.RegisterAssemblyTypes(typeof(INeptunInterface).GetTypeInfo().Assembly)
                   .Where(t => t.IsAssignableTo<ViewModelBase>())
                   .AsSelf();

            // data access classes
            // viewmodels
            builder.RegisterAssemblyTypes(typeof(INeptunInterface).GetTypeInfo().Assembly)
                   .Where(t => t.IsInNamespace($"{nameof(NeptunLight)}.{nameof(DataAccess)}"))
                   .AsImplementedInterfaces();

            // service implementations
            builder.RegisterType<InstanceDataStorage>()
                   .As<IDataStorage>()
                   .SingleInstance();

            builder.Register(context => MainActivity)
                   .As<INavigator>();

            return builder.Build();
        });
    }
}