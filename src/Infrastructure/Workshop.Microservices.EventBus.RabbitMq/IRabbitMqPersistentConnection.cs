using System;
using RabbitMQ.Client;

namespace Workshop.Microservices.EventBus.RabbitMq
{
    public interface IRabbitMqPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
