using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workshop.Microservices.EventBus;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.EventBus.RabbitMq;

namespace Workshop.Microservices.Extensions
{
    public static class EventBusExtensions
    {
        public static void RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMq(rabbitMqPersistentConnection,
                                            iLifetimeScope,
                                            eventBusSubscriptionsManager,
                                            subscriptionClientName,
                                            retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}