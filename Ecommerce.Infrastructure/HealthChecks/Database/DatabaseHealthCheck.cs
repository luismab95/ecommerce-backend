using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Infrastructure.Data;

namespace Ecommerce.Infrastructure.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly DatabaseMetrics _metrics;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseHealthCheck(IServiceProvider serviceProvider, DatabaseMetrics metrics)
    {
        _serviceProvider = serviceProvider;
        _metrics = metrics;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = HealthStatus.Healthy;

            // Obtener métricas actuales
            var metrics = _metrics.GetMetrics();
            var issues = EvaluateMetricThresholds(ref status);

            // Validar conectividad DB
            var dbStatus = await CheckDatabaseConnectivityAsync(cancellationToken);

            if (!dbStatus.CanConnect)
            {
                status = HealthStatus.Unhealthy;
                issues.Add("No se puede conectar a la base de datos");
            }

            // Construcción del objeto de salida
            var data = new Dictionary<string, object>(metrics)
            {
                ["db_connection"] = dbStatus.CanConnect,
                ["db_exception"] = dbStatus.Exception?.Message ?? "",
                ["status"] = status.ToString(),
                ["checked_at_utc"] = DateTime.UtcNow
            };

            // Resolver resultado final
            return status switch
            {
                HealthStatus.Healthy => HealthCheckResult.Healthy(
                    "Base de datos dentro de los parámetros normales",
                    data: data),

                HealthStatus.Degraded => HealthCheckResult.Degraded(
                    $"Advertencias: {string.Join(", ", issues)}",
                    data: data),

                _ => HealthCheckResult.Unhealthy(
                    $"Problemas detectados: {string.Join(", ", issues)}",
                    data: data)
            };
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"Error inesperado al verificar métricas",
                ex);
        }
    }


    // -----------------------------------------------------------------------
    // MÉTODOS AUXILIARES
    // -----------------------------------------------------------------------

    private List<string> EvaluateMetricThresholds(ref HealthStatus status)
    {
        var issues = new List<string>();

        if (_metrics.ConnectionCount > 100)
        {
            status = HealthStatus.Degraded;
            issues.Add($"Conexiones altas: {_metrics.ConnectionCount}");
        }

        if (_metrics.AvgQueryTime > TimeSpan.FromSeconds(2))
        {
            status = HealthStatus.Degraded;
            issues.Add($"Promedio de queries lento: {_metrics.AvgQueryTime.TotalMilliseconds}ms");
        }

        if (_metrics.ErrorRate > 0.10)
        {
            status = HealthStatus.Degraded;
            issues.Add($"Tasa de error alta: {_metrics.ErrorRate:P2}");
        }

        return issues;
    }

    private async Task<(bool CanConnect, Exception? Exception)> CheckDatabaseConnectivityAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            return (canConnect, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }
}
