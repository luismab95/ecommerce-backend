using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Infrastructure.HealthChecks;

public class DatabaseMetricsCollector : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseMetrics _metrics;

    private readonly TimeSpan _collectionInterval = TimeSpan.FromSeconds(30);


    public DatabaseMetricsCollector(
        IConfiguration configuration,
        DatabaseMetrics metrics
     )
    {
        _configuration = configuration;
        _metrics = metrics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CollectMetricsAsync(stoppingToken);
                await Task.Delay(_collectionInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Esperar más en caso de error
            }
        }

    }

    private async Task CollectMetricsAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            // Obtener número de conexiones
            await using var connectionCountCommand = new SqlCommand(@"
                SELECT COUNT(*) as ConnectionCount
                FROM sys.dm_exec_sessions 
                WHERE database_id = DB_ID() 
                AND is_user_process = 1", connection);

            var connectionCount = await connectionCountCommand.ExecuteScalarAsync(cancellationToken);
            _metrics.SetConnectionCount(Convert.ToInt32(connectionCount));

            // Medir tiempo de una query simple
            var startTime = DateTime.UtcNow;
            await using var testCommand = new SqlCommand("SELECT 1", connection);
            var result = await testCommand.ExecuteScalarAsync(cancellationToken);
            var duration = DateTime.UtcNow - startTime;

            _metrics.RecordQuery(duration, success: result != null);
        }
        catch (Exception)
        {
            _metrics.RecordQuery(TimeSpan.Zero, success: false);
        }
    }
}

