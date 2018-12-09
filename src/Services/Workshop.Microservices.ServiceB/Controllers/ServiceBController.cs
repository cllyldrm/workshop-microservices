using Microsoft.AspNetCore.Mvc;

namespace Workshop.Microservices.ServiceB.Controllers
{
    [Route("api/service-b")]
    [ApiController]
    public class ServiceBController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "returning from service B"
            });
        }
    }
}