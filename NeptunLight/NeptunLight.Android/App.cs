using System;
using Android.App;
using Android.Runtime;
using Autofac;
using NeptunLight.DataAccess;
using NeptunLight.Droid.Services;
using NeptunLight.Services;

namespace NeptunLight.Droid
{
    [Application]
    public class App : Application
    {
        private static readonly Lazy<IContainer> _container = new Lazy<IContainer>(() =>
        {
            ContainerBuilder builder = new ContainerBuilder();

            // built in classes
            builder.RegisterModule<AutofacModuleDefinition>();

            // service implementations
            builder.RegisterType<FileDataStorage>()
                   .As<IDataStorage>()
                   .SingleInstance();

            builder.RegisterType<MailContentCache>()
                   .As<IMailContentCache>()
                   .SingleInstance();

            builder.RegisterType<PrimitiveStorage>()
                   .As<IPrimitiveStorage>()
                   .SingleInstance();

            builder.RegisterType<RefreshManager>()
                   .As<IRefreshManager>()
                   .SingleInstance();

            builder.RegisterType<NotificationService>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<AndroidWebScraperClient>()
                .As<WebScraperClient>()
                .InstancePerDependency();


            builder.Register(context => MainActivity)
                   .As<INavigator>();

            return builder.Build();
        });

        protected App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public static MainActivity MainActivity { get; set; }

        public static IContainer Container => _container.Value;
    }
}