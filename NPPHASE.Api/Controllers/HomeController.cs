using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        private readonly IActionResult _result;

        public HomeController(IHomeService deviceService, IExceptionLoggerServices exceptionLoggerServices)
        {
            
        }
        

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("s");
        }

        [HttpGet("Test2")]
        public IActionResult test2()
        {
            return Ok("s");
        }
    }
}
