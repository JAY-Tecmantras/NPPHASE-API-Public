using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAnalyTicsDetails()
        {
            return new OkObjectResult(new ResponseMessageViewModel
            {
                Data = await _analyticsService.GetAnalyTicsDetails(),
                IsSuccess = true
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAnalyTicsDetailsByYear(int year)
        {
            return new OkObjectResult(new ResponseMessageViewModel
            {
                Data = await _analyticsService.GetAnalyTicsDetailsByYear(year),
                IsSuccess = true
            });
        }

    }
}
