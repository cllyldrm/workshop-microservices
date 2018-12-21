using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workshop.Microservices.Domain.Events;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.ServiceA.Commands;
using Workshop.Microservices.ServiceA.Events;

namespace Workshop.Microservices.ServiceA.Controllers
{
    [Route("api/service-a")]
    [ApiController]
    public class ServiceAController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        private readonly IMediator _mediator;

        public ServiceAController(IEventBus eventBus, IMediator mediator)
        {
            _eventBus = eventBus;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _mediator.Publish(new AEvent("sending notification from service A"));

            await _mediator.Send(new ACommand("sending command from service A"));

            _eventBus.Publish(new SendInformationToServiceBEvent("sending event by event bus from service A"));

            return Ok(new
            {
                message = "returning from service A"
            });
        }
    }
}