using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ecommerce.Infrastructure.HealthChecks;


public class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var memoryMB = process.WorkingSet64 / 1024 / 1024;
            var maxMemoryMB = 1024; // 1GB límite

            var data = new Dictionary<string, object>
            {
                ["memory_mb"] = memoryMB,
                ["max_memory_mb"] = maxMemoryMB,
                ["percentage"] = (double)memoryMB / maxMemoryMB * 100,
                ["process"] = process.ProcessName
            };

            if (memoryMB > maxMemoryMB * 0.9) // > 90%
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Memoria alta: {memoryMB}MB",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Memoria OK: {memoryMB}MB",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Error al verificar memoria",
                exception: ex));
        }
    }
}