using Microsoft.Azure.Cosmos;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (Static Web App)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cosmos DB
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var endpoint = config["CosmosDb:AccountEndpoint"];
    var key = config["CosmosDb:AccountKey"];

    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
        throw new InvalidOperationException("Cosmos DB config missing");

    return new CosmosClient(endpoint, key);
});

// DI
builder.Services.AddScoped<INoteRepository, CosmosNoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();

var app = builder.Build();

// ✅ Swagger (DEV ONLY)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
