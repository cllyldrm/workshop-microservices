using MediatR;

namespace Workshop.Microservices.ServiceA.Events
{
    public class AEvent : INotification
    {
        public string Information { get; private set; }

        public AEvent(string information)
        {
            Information = information;
        }
    }
}
