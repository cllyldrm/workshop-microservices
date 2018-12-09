using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.Extensions;
using Workshop.Microservices.ServiceB.IntegrationEvents.EventHandling;
using Workshop.Microservices.ServiceB.IntegrationEvents.Events;

namespace Workshop.Microservices.ServiceB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.RegisterRabbitMq(Configuration);
            services.RegisterEventBus(Configuration);

            services.AddSingleton<SendInformationToServiceBEventHandler>();

            return services.BuildWithAutoFac();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<SendInformationToServiceBEvent, SendInformationToServiceBEventHandler>();
        }
    }
}