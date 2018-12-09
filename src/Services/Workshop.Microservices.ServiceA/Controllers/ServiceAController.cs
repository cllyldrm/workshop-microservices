using Microsoft.AspNetCore.Mvc;

namespace Workshop.Microservices.ServiceA.Controllers
{
    [Route("api/service-a")]
    [ApiController]
    public class ServiceAController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "returning from service A"
            });
        }
    }
}