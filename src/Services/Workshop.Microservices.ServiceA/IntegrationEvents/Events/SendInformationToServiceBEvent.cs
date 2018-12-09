using Workshop.Microservices.EventBus.Events;

namespace Workshop.Microservices.ServiceA.IntegrationEvents.Events
{
    public class SendInformationToServiceBEvent : IntegrationEvent
    {
        public string Information { get; set; }

        public SendInformationToServiceBEvent(string information)
        {
            Information = information;
        }
    }
}