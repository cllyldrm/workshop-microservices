using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Workshop.Microservices.EventBus.RabbitMq;

namespace Workshop.Microservices.Extensions
{
    public static class RabbitMqExtensions
    {
        public static void RegisterRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMqPersistentConnection(factory, retryCount);
            });
        }
    }
}