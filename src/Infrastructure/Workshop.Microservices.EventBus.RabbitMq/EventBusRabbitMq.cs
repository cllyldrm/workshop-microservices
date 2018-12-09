using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.EventBus.Events;

namespace Workshop.Microservices.EventBus.RabbitMq
{
    public class EventBusRabbitMq : IEventBus, IDisposable
    {
        private const string BrokerName = "workshop-microservices_event_bus";
        private const string AutoFacScopeName = "workshop-microservices_event_bus";

        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autoFac;
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private readonly string _queueName;

        public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
                                ILifetimeScope autoFac,
                                IEventBusSubscriptionsManager subsManager,
                                string queueName = null,
                                int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autoFac = autoFac;
            _retryCount = retryCount;
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                               .Or<SocketException>()
                               .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => { });

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType()
                                      .Name;

                channel.ExchangeDeclare(exchange: BrokerName,
                                        type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;

                    channel.BasicPublish(exchange: BrokerName,
                                         routingKey: eventName,
                                         mandatory: true,
                                         basicProperties: properties,
                                         body: body);
                });
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            _subsManager.AddSubscription<T, TH>();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                                      exchange: BrokerName,
                                      routingKey: eventName);
                }
            }
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();

            _subsManager.Clear();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BrokerName,
                                    type: "direct");

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autoFac.BeginLifetimeScope(AutoFacScopeName))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task) concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                        }
                    }
                }
            }
        }
    }
}