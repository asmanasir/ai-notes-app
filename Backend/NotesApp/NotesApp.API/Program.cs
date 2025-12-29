using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//
// ---------------- LOGGING ----------------
//
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//
// ---------------- CORS ----------------
// Allows:
// - localhost (development)
// - Azure Static Web App (production)
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://orange-rock-0ad77e71e3.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//
// ---------------- API ----------------
//
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();

//
// ---------------- SWAGGER ----------------
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// ---------------- APPLICATION LAYER ----------------
//
builder.Services.AddScoped<INotesService, NotesService>();

//
// ---------------- DATABASE (SQL SERVER) ----------------
// Connection string comes from:
// - User Secrets (local)
// - Azure App Service configuration (prod)
//
builder.Services.AddDbContext<NotesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        });
});

builder.Services.AddScoped<INoteRepository, SqlNoteRepository>();

//
// ---------------- AI SERVICES ----------------
//
builder.Services.AddScoped<IAiService, AiService>();

//
// ---------------- BUILD APP ----------------
//
var app = builder.Build();

//
// ---------------- AUTO-MIGRATION (SAFE) ----------------
// Applies pending EF Core migrations on startup
//
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
    db.Database.Migrate();
}

//
// ---------------- MIDDLEWARE ----------------
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
