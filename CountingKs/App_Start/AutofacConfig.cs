using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using CountingKs.Data;
using CountingKs.Services;
using System.Reflection;
using System.Web.Http;

namespace CountingKs.App_Start
{
    public static class AutofacConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            RegisterDataAccessTypes(builder);

            builder.RegisterHttpRequestMessage(GlobalConfiguration.Configuration);

            RegisterServices(builder);

            RegisterControllers(builder);

            return SetDependencyResolver(builder);
        }

        private static void RegisterDataAccessTypes(ContainerBuilder builder)
        {
            builder.RegisterInstance(new CountingKsContext()).AsSelf().SingleInstance();
            builder.RegisterType<CountingKsRepository>().As<ICountingKsRepository>().SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<CountingKsIdentityService>().As<ICountingKsIdentityService>().SingleInstance();
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            var assemblyWithControllers = Assembly.GetExecutingAssembly();
            builder.RegisterApiControllers(assemblyWithControllers);
            builder.RegisterControllers(assemblyWithControllers);
        }

        private static IContainer SetDependencyResolver(ContainerBuilder builder)
        {
            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }
    }
}