using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Extensions;
using Ecommerce.Domain.DTOs.Auth;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Domain.Interfaces.Services;
using Ecommerce.Infrastructure.Repositories;
using Ecommerce.Infrastructure.Services;
using Ecommerce.Infrastructure.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog;

namespace Ecommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder host,
        IHostEnvironment environment)
    {
        // Configuración de SQL Server
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositorios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Servicios
        services.AddScoped<IAuthService, JwtAuthService>();
        services.AddScoped<IEmailService, GmailSmtpEmailSender>();
        services.AddScoped<IUploadImageService, LocalStorageImageService>();


        // Configurar JWT
        services.Configure<JwtResponse>(configuration.GetSection("JwtSettings"));
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();

        // cors
        services.AddCors(options =>
        {
            options.AddPolicy("NewPolicy", app =>
            {
                app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        // Health Checks
        services.AddSingleton<DatabaseMetrics>();
        services.AddHostedService<DatabaseMetricsCollector>();

        services.AddHealthChecks()
            .AddSqlServer(
                connectionString: connectionString!,
                healthQuery: "SELECT 1;",
                name: "sqlServer",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "database", "sqlserver" })
            .AddCheck<DatabaseHealthCheck>(
                "databaseContext",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "database", "metrics", "context" })
            .AddCheck<MemoryHealthCheck>(
                "memory",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "system", "memory" });


        // Serilog
        host.UseSerilog((context, services, logConfg) =>
        {
            logConfg
                .ReadFrom.Configuration(context.Configuration);
        });


        // OpenTelemetry
        // OrderUseCases
        services.AddOpenTelemetry()
         .ConfigureResource(resource => resource.AddService("OrderUseCases"))
         .WithTracing(tracing =>
         {
             var jaegerUri = configuration["Jaeger:Uri"] ?? "";
             tracing
                 .AddSource("OrderUseCases")
                 .AddAspNetCoreInstrumentation(options =>
                 {
                     options.RecordException = true;
                     options.EnrichWithHttpRequest = (activity, request) =>
                     {
                         activity.SetTag("http.request.method", request.Method);
                         activity.SetTag("http.client_ip", request.HttpContext.Connection.RemoteIpAddress?.ToString());
                         activity.SetTag("http.request.content_length", request.ContentLength);
                     };
                     options.EnrichWithHttpResponse = (activity, response) =>
                     {
                         activity.SetTag("http.response.status_code", response.StatusCode);
                         activity.SetTag("http.response.content_length", response.ContentLength);
                     };

                     // Excluir swagger y health
                     options.Filter = context =>
                         !context.Request.Path.Value!.Contains("swagger", StringComparison.OrdinalIgnoreCase) &&
                         !context.Request.Path.Value.Contains("health", StringComparison.OrdinalIgnoreCase);
                 })
                 .AddHttpClientInstrumentation(options =>
                 {
                     options.RecordException = true;
                     options.EnrichWithHttpRequestMessage = (activity, request) =>
                     {
                         activity.SetTag("http.request.method", request.Method.Method);
                         activity.SetTag("http.request.url", request.RequestUri?.ToString());
                     };
                 })
                 .AddSqlClientInstrumentation(options =>
                 {
                     options.RecordException = true;
                 })
                 .AddOtlpExporter(otlp =>
                 {
                     otlp.Endpoint = new Uri(jaegerUri);
                     otlp.Protocol = OtlpExportProtocol.Grpc;
                     otlp.ExportProcessorType = ExportProcessorType.Batch;
                     otlp.BatchExportProcessorOptions = new()
                     {
                         MaxQueueSize = 2048,
                         ScheduledDelayMilliseconds = 3000,
                         ExporterTimeoutMilliseconds = 10000,
                         MaxExportBatchSize = 256
                     };
                 });
         })
         .WithMetrics(metrics =>
         {
             metrics
                 .AddMeter("OrderUseCases")
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation()
                 .AddRuntimeInstrumentation()
                 .AddProcessInstrumentation();
         });

        return services;
    }
}