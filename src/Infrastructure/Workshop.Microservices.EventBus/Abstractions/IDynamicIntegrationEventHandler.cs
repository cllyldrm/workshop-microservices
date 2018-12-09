using System.Threading.Tasks;

namespace Workshop.Microservices.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
