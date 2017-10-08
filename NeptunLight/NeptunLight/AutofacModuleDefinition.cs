using Autofac;
using NeptunLight.DataAccess;
using NeptunLight.ViewModels;
using System.Reflection;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace NeptunLight
{
    public class AutofacModuleDefinition : Module
    {
        protected override void Load([NotNull] ContainerBuilder builder)
        {
            base.Load(builder);
            // viewmodels
            builder.RegisterAssemblyTypes(typeof(INeptunInterface).GetTypeInfo().Assembly)
                   .Where(t => t.IsAssignableTo<ViewModelBase>())
                   .AsSelf();

            // data access classes
            builder.RegisterType<WebNeptunInterface>()
                   .As<INeptunInterface>()
                   .SingleInstance();
            builder.RegisterType<StaticData>()
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
