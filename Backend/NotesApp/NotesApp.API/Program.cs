using Microsoft.Azure.Cosmos;
using NotesApp.Application.Interfaces;
using NotesApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Controllers & Swagger
// ==========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==========================
// CORS (VERY IMPORTANT)
// ==========================
var allowedOrigins = builder.Configuration["AllowedOrigins"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (!string.IsNullOrWhiteSpace(allowedOrigins))
        {
            policy.WithOrigins(allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // fallback (useful for local testing)
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// ==========================
// Cosmos DB
// ==========================
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var endpoint = config["CosmosDb:AccountEndpoint"];
    var key = config["CosmosDb:AccountKey"];

    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
        throw new InvalidOperationException("Cosmos DB configuration is missing");

    return new CosmosClient(endpoint, key);
});

// ==========================
// Repositories
// ==========================
builder.Services.AddScoped<INoteRepository, CosmosNoteRepository>();

var app = builder.Build();

// ==========================
// Middleware pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⬇️ CORS MUST be before MapControllers
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
