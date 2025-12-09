using Ecommerce.Domain.DTOs.General;
using Ecommerce.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/health")]
    public class HealthCheckController : ControllerBase
    {

        private readonly HealthCheckService _healthCheckService;
        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("metrics")]
        public Task<IActionResult> GetMetrics(DatabaseMetrics metrics)
        {

            var response = new GeneralResponse
            {
                Data = new
                {
                    timestamp = DateTime.UtcNow,
                    application = "Ecommerce",
                    version = "1.0.0",
                    metrics = metrics.GetMetrics()
                },
                Message = "Proceso realizado con éxito."
            };
            return Task.FromResult<IActionResult>(Ok(response));



        }

        [HttpGet("")]
        public async Task<IActionResult> GetHealthChecks()
        {

            var report = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds,
                    exception = e.Value.Exception?.Message
                }),
                timestamp = DateTime.UtcNow,
                totalDuration = report.TotalDuration.TotalMilliseconds
            };

            return Ok(new GeneralResponse
            {
                Data = response,
                Message = "Proceso realizado con éxito."
            });


        }
    }
}
