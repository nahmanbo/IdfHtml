using Microsoft.AspNetCore.Mvc;
using IdfOperation.Web.Services;

namespace IdfOperation.Web.Controllers
{
    [ApiController]
    [Route("api/operation")]
    public class OperationController : ControllerBase
    {
        private readonly OperationService _service;

        public OperationController(OperationService service)
        {
            _service = service;
        }

        [HttpPost("option/{id}")]
        public async Task<IActionResult> HandleOption(int id)
        {
            try
            {
                string body = "";
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                string result = id switch
                {
                    1 => _service.ViewFullIdfInfo(),
                    2 => _service.ViewFullHamasInfo(),
                    3 => _service.ViewFirepowerData(),
                    4 => _service.ViewIntelligenceReports(),
                    5 => !string.IsNullOrWhiteSpace(body) ? _service.ViewReportByName(body) : "Missing terrorist name",
                    6 => _service.ViewMostDangerous(),
                    7 => !string.IsNullOrWhiteSpace(body) ? _service.EliminateByName(body) : "Missing terrorist name",
                    8 => _service.EliminateMostDangerous(),
                    9 => !string.IsNullOrWhiteSpace(body) ? _service.EliminateByTargetType(body) : "Missing target type",
                    _ => "Invalid option"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
