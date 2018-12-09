using System;
using System.Threading.Tasks;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.ServiceB.IntegrationEvents.Events;

namespace Workshop.Microservices.ServiceB.IntegrationEvents.EventHandling
{
    public class SendInformationToServiceBEventHandler : IIntegrationEventHandler<SendInformationToServiceBEvent>
    {
        public async Task Handle(SendInformationToServiceBEvent @event)
        {
            Console.WriteLine($"Event Id: {@event.Id} Creation Date: {@event.CreationDate} Message: {@event.Information}");

            await Task.CompletedTask;
        }
    }
}