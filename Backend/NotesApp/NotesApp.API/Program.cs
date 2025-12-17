using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Azure.Cosmos;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cosmos Client (singleton)
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var endpoint = config["CosmosDb:AccountEndpoint"];
    var key = config["CosmosDb:AccountKey"];

    if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key))
        throw new InvalidOperationException("Cosmos DB configuration is missing.");

    return new CosmosClient(endpoint, key);
});

// Repositories & Services
builder.Services.AddScoped<INoteRepository, CosmosNoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();

// CORS (frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Forwarded headers (Azure-safe)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
