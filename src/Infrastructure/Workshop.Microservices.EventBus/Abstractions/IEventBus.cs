using Workshop.Microservices.EventBus.Events;

namespace Workshop.Microservices.EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
