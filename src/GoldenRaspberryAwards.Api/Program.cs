using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Services;
using GoldenRaspberryAwards.Api.Modules.Awards.Application.Handlers;
using GoldenRaspberryAwards.Api.Modules.Awards.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AwardsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=:memory:"));

builder.Services.AddScoped<ProducerAwardEventProcessor>();
builder.Services.AddScoped<CsvImportService>();

var app = builder.Build();

await SeedInitialData(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapAwardsEndpoints();

app.Run();

static async Task SeedInitialData(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AwardsDbContext>();

    db.Database.OpenConnection();
    db.Database.EnsureCreated();

    var csvPath = app.Configuration["Awards:MovieListPath"] ?? throw new InvalidOperationException("MovieListPath configuration is missing");
    var importService = scope.ServiceProvider.GetRequiredService<CsvImportService>();
    await importService.ImportFromCsvAsync(csvPath);
}

public partial class Program { }
