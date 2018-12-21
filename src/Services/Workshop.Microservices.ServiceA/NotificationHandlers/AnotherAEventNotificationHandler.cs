using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Workshop.Microservices.ServiceA.Events;

namespace Workshop.Microservices.ServiceA.NotificationHandlers
{
    public class AnotherAEventNotificationHandler : INotificationHandler<AEvent>
    {
        public Task Handle(AEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"AnotherAEventNotificationHandler Information: {notification.Information} ");

            return Task.FromResult(0);
        }
    }
}