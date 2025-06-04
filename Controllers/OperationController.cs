using Microsoft.AspNetCore.Mvc;
using IdfOperation.Web.Services;
using System.Text.Json;

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

                    5 => int.TryParse(body, out var reportId)
                        ? _service.ViewReportById(reportId)
                        : "❌ Missing or invalid terrorist ID",

                    6 => _service.ViewMostDangerous(),

                    7 => int.TryParse(body, out var elimId)
                        ? _service.EliminateById(elimId)
                        : "❌ Missing or invalid terrorist ID",

                    8 => _service.EliminateMostDangerous(),

                    9 => !string.IsNullOrWhiteSpace(body)
                        ? _service.EliminateByTargetType(body.Trim())
                        : "❌ Missing target type",

                    10 => _service.ExecuteStrikeWithAmmo(JsonSerializer.Deserialize<StrikePayload>(body)),

                    _ => "❌ Invalid option"
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
