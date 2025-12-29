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
// IMPORTANT:
// - Must match frontend EXACTLY
// - Must be applied BEFORE controllers
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://orange-rock-0ad77e71e.3.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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
// ---------------- APPLICATION ----------------
//
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<INoteRepository, SqlNoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();

//
// ---------------- DATABASE ----------------
//
builder.Services.AddDbContext<NotesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql =>
        {
            sql.EnableRetryOnFailure();
        });
});

var app = builder.Build();

//
// ---------------- CORS MUST BE FIRST ----------------
// 🔴 THIS IS CRITICAL
//
app.UseCors("AllowFrontend");

//
// ---------------- AUTO MIGRATION ----------------
//
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
    db.Database.Migrate();
}

//
// ---------------- SWAGGER ----------------
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//
// ---------------- ENDPOINTS ----------------
//
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
