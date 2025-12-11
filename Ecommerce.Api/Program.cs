using Ecommerce.Infrastructure.Configurations;
using Ecommerce.Infrastructure;
using Ecommerce.Application;
using Ecommerce.Api.Filters;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Capa de Aplicación
builder.Services.AddApplication();

// Capa de Infraestructura
builder.Services.AddInfrastructure(builder.Configuration, builder.Host, builder.Environment);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCustomInvalidModelStateResponse();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.HttpOnly = false;
    options.Cookie.Path = "/";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy =
        builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always; // producción HTTPS
    options.HeaderName = "X-XSRF-TOKEN";
});



// Filters
builder.Services.AddScoped<PostAuthorizeFilter>();
builder.Services.AddScoped<PostAuthorizeRoleFilter>();


// Servicios necesarios para Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.OperationFilter<SwaggerAuthorizeOperationFilter>();
});


var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorResponseLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Genera el JSON OpenAPI
    app.UseSwaggerUI(); // Genera la UI Swagger
}


app.UseCors("FrontEndPolicy");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

