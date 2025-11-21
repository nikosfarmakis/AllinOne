using AllinOne.FilterAttributes;
using Microsoft.AspNetCore.Mvc;

namespace AllinOne.Controllers
{
    [RateLimitingFilter(20, expirationInSeconds: 20)]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<AllinOneController> _logger;

        public PersonController(ILogger<AllinOneController> logger) 
        { 
            _logger = logger;
        }
    }
}
