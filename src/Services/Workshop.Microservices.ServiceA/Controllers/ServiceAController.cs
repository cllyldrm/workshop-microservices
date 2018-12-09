using System;
using Microsoft.AspNetCore.Mvc;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.ServiceA.IntegrationEvents.Events;

namespace Workshop.Microservices.ServiceA.Controllers
{
    [Route("api/service-a")]
    [ApiController]
    public class ServiceAController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public ServiceAController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Console.WriteLine("Sending event by event bus to rabbitmq");

            _eventBus.Publish(new SendInformationToServiceBEvent("sending from service A"));

            Console.WriteLine("Sent event by event bus to rabbitmq");

            return Ok(new
            {
                message = "returning from service A"
            });
        }
    }
}