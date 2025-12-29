using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ LOGGING FIRST
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Azure App Configuration
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    var settings = config.Build();
    var appConfigConnection = settings["AppConfig:ConnectionString"];

    if (!string.IsNullOrEmpty(appConfigConnection))
    {
        config.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(appConfigConnection)
                .UseFeatureFlags()
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("Sentinel", refreshAll: true)
                           .SetCacheExpiration(TimeSpan.FromSeconds(30));
                });
        });
    }
});

builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();


// DB + repos here...

var app = builder.Build();

// Middleware
app.UseAzureAppConfiguration();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

