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

        //====================================
        public OperationController(OperationService service)
        {
            _service = service;
        }

        //--------------------------------------------------------------
        [HttpPost("option/{id}")]
        public async Task<IActionResult> HandleOption(int id)
        {
            try
            {
                var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

                var result = id switch
                {
                    1 => _service.ViewFullIdfInfo(),
                    2 => _service.ViewFullHamasInfo(),
                    3 => _service.ViewFirepowerData(),
                    4 => _service.ViewIntelligenceReports(),

                    5 => ParseIdAndExecute(requestBody, _service.ViewReportById, "❌ Missing or invalid terrorist ID"),
                    6 => _service.ViewMostDangerous(),
                    7 => ParseIdAndExecute(requestBody, _service.EliminateById, "❌ Missing or invalid terrorist ID"),
                    8 => _service.EliminateMostDangerous(),
                    9 => string.IsNullOrWhiteSpace(requestBody)
                        ? "❌ Missing target type"
                        : _service.EliminateByTargetType(requestBody.Trim()),
                    10 => _service.ExecuteStrikeWithAmmo(JsonDocument.Parse(requestBody).RootElement),
                    _ => "❌ Invalid option"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        //--------------------------------------------------------------
        private static string ParseIdAndExecute(string input, Func<int, string> action, string errorMessage)
        {
            return int.TryParse(input, out var id) ? action(id) : errorMessage;
        }
    }
}
