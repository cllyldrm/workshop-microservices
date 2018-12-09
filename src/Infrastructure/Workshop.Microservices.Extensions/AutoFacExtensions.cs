using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Workshop.Microservices.Extensions
{
    public static class AutoFacExtensions
    {
        public static IServiceProvider BuildWithAutoFac(this IServiceCollection services)
        {
            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }
    }
}