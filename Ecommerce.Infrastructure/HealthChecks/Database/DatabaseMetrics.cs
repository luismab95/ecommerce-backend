namespace Ecommerce.Infrastructure.HealthChecks;

public class DatabaseMetrics
{
    private readonly object _lock = new();
    private readonly Queue<TimeSpan> _queryTimes = new();
    private readonly Queue<bool> _queryResults = new();
    private const int MaxSamples = 100;

    public int ConnectionCount { get; private set; }
    public TimeSpan AvgQueryTime { get; private set; }
    public double ErrorRate { get; private set; }
    public long TotalQueries { get; private set; }
    public long FailedQueries { get; private set; }

    public void RecordQuery(TimeSpan duration, bool success)
    {
        lock (_lock)
        {
            _queryTimes.Enqueue(duration);
            _queryResults.Enqueue(success);

            // Mantener solo los últimos MaxSamples
            while (_queryTimes.Count > MaxSamples) _queryTimes.Dequeue();
            while (_queryResults.Count > MaxSamples) _queryResults.Dequeue();

            // Calcular métricas
            AvgQueryTime = _queryTimes.Any()
                ? TimeSpan.FromMilliseconds(_queryTimes.Average(t => t.TotalMilliseconds))
                : TimeSpan.Zero;

            ErrorRate = _queryResults.Any()
                ? (double)_queryResults.Count(r => !r) / _queryResults.Count
                : 0;

            TotalQueries++;
            if (!success) FailedQueries++;
        }
    }

    public void SetConnectionCount(int count)
    {
        ConnectionCount = count;
    }

    public Dictionary<string, object> GetMetrics()
    {
        return new Dictionary<string, object>
        {
            ["connection_count"] = ConnectionCount,
            ["avg_query_time_ms"] = AvgQueryTime.TotalMilliseconds,
            ["error_rate"] = ErrorRate,
            ["total_queries"] = TotalQueries,
            ["failed_queries"] = FailedQueries,
            ["sample_size"] = _queryTimes.Count,
            ["last_updated"] = DateTime.UtcNow
        };
    }
}