// Program.cs
using HbysApi.Application;
using HbysApi.Infrastructure;
using HbysApi.Infrastructure.Repositories;
using Serilog;
using HbysApi.Infrastructure.Redis;
using HbysApi.WebApi.Endpoints;
using HbysApi.WebApi.Middleware;

// Program entry point and configuration
// Program giriş noktası ve yapılandırma
var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
// Loglama için Serilog'u yapılandır
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(outputTemplate: "[{Timestamp:dd.MM.yyyy HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, 
        outputTemplate: "[{Timestamp:dd.MM.yyyy HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add configuration files
// Konfigürasyon dosyalarını ekle
builder.Configuration.AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Register Oracle repository as singleton
// Oracle repository'sini singleton olarak kaydet
builder.Services.AddSingleton(new Repository(
    builder.Configuration
    .GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection", "Connection string cannot be null.")));

builder.Services.AddInfrastructure(builder.Configuration);

// Register Redis with TLS using extension method
// Redis'i TLS ile extension method kullanarak kaydet
builder.Services.AddRedisWithTls(builder.Configuration);

builder.Services.AddControllers(); // Controller'ları eklediğinizden emin olun

var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();

// Global Exception Handler (Production)
if (!app.Environment.IsDevelopment())
{
    app.UseGlobalExceptionHandler();
}

// Log request start for each HTTP request
// Her HTTP isteği için başlangıç logunu yaz
app.Use(async (context, next) =>
{
    Log.Information("---------- [REQUEST START] ----------");
    await next();
});

// Register minimal API endpoints
// Minimal API endpointlerini kaydet
app.MapRedisEndpoints();

// Health Check Endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();
