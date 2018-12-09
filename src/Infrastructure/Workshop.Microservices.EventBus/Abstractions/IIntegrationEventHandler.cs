using System.Threading.Tasks;
using Workshop.Microservices.EventBus.Events;

namespace Workshop.Microservices.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}