using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Azure.Cosmos;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Controllers & Swagger
// ===============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===============================
// Configuration
// ===============================
var configuration = builder.Configuration;

// ===============================
// Cosmos DB
// ===============================
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var endpoint = configuration["CosmosDb:AccountEndpoint"];
    var key = configuration["CosmosDb:AccountKey"];

    if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key))
    {
        throw new InvalidOperationException("Cosmos DB configuration is missing.");
    }

    return new CosmosClient(endpoint, key);
});

builder.Services.AddScoped<INoteRepository, CosmosNoteRepository>();

// ===============================
// AI Service
// ===============================
builder.Services.AddScoped<IAiService, AiService>();

// ===============================
// CORS (Azure Static Web App)
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "https://orange-rock-0ad77e71e.3.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

// ===============================
// Forwarded Headers (Azure-safe)
// ===============================
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// ===============================
// Middleware order (IMPORTANT)
// ===============================
app.UseForwardedHeaders();

app.UseCors("AllowFrontend"); // 🔥 MUST be before MapControllers

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
