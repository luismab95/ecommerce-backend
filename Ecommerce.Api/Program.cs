using Ecommerce.Infrastructure;
using Ecommerce.Api.Filters;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


// Capa de Infraestructura
builder.Services.AddInfrastructure(builder.Configuration, builder.Host, builder.Environment);


// Add services to the container.
builder.Services.AddControllers();
// builder.Services.AddCustomInvalidModelStateResponse();


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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Genera el JSON OpenAPI
    app.UseSwaggerUI(); // Genera la UI Swagger
}

app.UseCors("NewPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

