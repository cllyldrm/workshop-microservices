# Microservices Training

Sample microservice architecture on .net core 2.1 with rabbitMQ, mediatR and eventbus, also included ocelot api gateway and docker compose.

## Getting Started

### Using

	docker-compose up
  
## Api Gateway

There is a json file which name is ocelot. You can find routing and aggregation rules in it. 

- **UpstreamPathTemplate** - Api gateway path.
- **DownstreamPathTemplate** - Where to send.
- **Aggregates** - Combines two or more service response into one call.

````json
{
  "ReRoutes": [
    {
      "DownstreamPathTemplate": "/api/service-a",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "servicea",
          "Port": 5001
        }
      ],
      "UpstreamPathTemplate": "/api/a",
      "Key": "ServiceA"
    },
    {
      "DownstreamPathTemplate": "/api/service-b",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "serviceb",
          "Port": 5002
        }
      ],
      "UpstreamPathTemplate": "/api/b",
      "Key": "ServiceB"
    }
  ],
  "Aggregates": [
    {
      "ReRouteKeys": [
        "ServiceA",
        "ServiceB"
      ],
      "UpstreamPathTemplate": "/api/a-b"
    }
  ],
  "GlobalConfiguration": {
    "RequestIdKey": "OcRequestId"
  }
}
````
 
## EventBus Publishing Event

There is an abstraction for event bus and rabbitmq client. Inspired from [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers).
 
If you want to send an event to another microservices, first create an integration event.
 
````csharp
    public class SendInformationToServiceBEvent : IntegrationEvent
    {
        public string Information { get; set; }

        public SendInformationToServiceBEvent(string information)
        {
            Information = information;
        }
    }
````
Just publish this event from event bus.

````csharp
    _eventBus.Publish(new SendInformationToServiceBEvent("sending from service A"));
````

## EventBus Subscribe Event
 
Create an event handler for subscription.
 
````csharp
    public class SendInformationToServiceBEventHandler : IIntegrationEventHandler<SendInformationToServiceBEvent>
    {
        public async Task Handle(SendInformationToServiceBEvent @event)
        {
            Console.WriteLine($"Event Id: {@event.Id} Creation Date: {@event.CreationDate} Message: {@event.Information}");

            await Task.CompletedTask;
        }
    }
````
Don't forget to register event handlers.
 
````csharp
    services.AddSingleton<SendInformationToServiceBEventHandler>();
````
  
Finally, subscribe an event handler from event bus.

````csharp
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseMvc();

        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        eventBus.Subscribe<SendInformationToServiceBEvent, SendInformationToServiceBEventHandler>();
    }
````

## Sending Commands with MediatR

Create an event with mediatR.

````csharp
    public class ACommand : IRequest<bool>
    {
        public string Information { get; private set; }

        public ACommand(string information)
        {
            Information = information;
        }
    }
````
Create a handler which is related with event. 

````csharp
    public class ACommandHandler : IRequestHandler<ACommand, bool>
    {
        public async Task<bool> Handle(ACommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"ACommandHandler Information: {request.Information} ");

            return true;
        }
    }
````
Send this command whatever you where.

````csharp
    await _mediator.Send(new ACommand("sending command from service A"));
````

## Sending Notifications with MediatR

Create a notification with mediatR.

````csharp
    public class AEvent : INotification
    {
        public string Information { get; private set; }

        public AEvent(string information)
        {
            Information = information;
        }
    }
````

Create handlers which are related with notification. 

````csharp
    public class AEventNotificationHandler : INotificationHandler<AEvent>
    {
        public Task Handle(AEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"AEventNotificationHandler Information: {notification.Information} ");

            return Task.FromResult(0);
        }
    }

    public class AnotherAEventNotificationHandler : INotificationHandler<AEvent>
    {
        public Task Handle(AEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"AnotherAEventNotificationHandler Information: {notification.Information} ");

            return Task.FromResult(0);
        }
    }
````

Send this command whatever you where.

````csharp
    await _mediator.Publish(new AEvent("sending notification from service A"));
````

Ta-da!!
