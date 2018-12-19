using System;
using System.Threading.Tasks;
using Workshop.Microservices.Domain.Events;
using Workshop.Microservices.EventBus.Abstractions;

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