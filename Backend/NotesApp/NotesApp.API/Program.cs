using Microsoft.Azure.Cosmos;
using NotesApp.Application.Interfaces;
using NotesApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Controllers
// =======================
builder.Services.AddControllers();

// =======================
// Swagger
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================
// CORS (FOR STATIC WEB APP)
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =======================
// Cosmos DB
// =======================
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var endpoint = config["CosmosDb:AccountEndpoint"];
    var key = config["CosmosDb:AccountKey"];

    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
        throw new InvalidOperationException("Cosmos DB config missing");

    return new CosmosClient(endpoint, key);
});

// =======================
// DI
// =======================
builder.Services.AddScoped<INoteRepository, CosmosNoteRepository>();

var app = builder.Build();

// =======================
// Middleware ORDER (CRITICAL)
// =======================

app.UseHttpsRedirection();

// 🔥 CORS MUST be here
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

// =======================
app.Run();
