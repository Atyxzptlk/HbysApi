// Program.cs
using HbysApi.Application;
using HbysApi.Infrastructure;
using HbysApi.Infrastructure.Repositories;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Explicitly add the appsettings.json file from the Config folder
builder.Configuration.AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add Oracle connection string as a singleton
builder.Services.AddSingleton(new Repository(
    builder.Configuration
    .GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection", "Connection string cannot be null.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.MapGet("/api/sample", async ([FromServices] Repository repository, IConfiguration configuration) =>
{
    var query = configuration["Queries:PhoneBookQuery"];

    if (string.IsNullOrEmpty(query))
    {
        return Results.BadRequest("Query is missing.");
    }

    try
    {
        var result = await repository.ExecuteQueryAsListAsync(query);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        // Hassas bilgileri loglamayın
        Console.WriteLine("An error occurred while executing the query."); // Genel bir hata mesajı
        return Results.Problem($"Internal server error: {ex.Message}");
    }
});

app.Run();
