using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//
// ---------- LOGGING ----------
//
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//
// ---------- CORS (ONCE ONLY) ----------
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
            .AllowAnyMethod();
    });
});

//
// ---------- API ----------
//
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();

//
// ---------- SWAGGER ----------
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// ---------- DATABASE ----------
//
builder.Services.AddDbContext<NotesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql =>
        {
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
});

//
// ---------- APPLICATION ----------
//
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<INoteRepository, SqlNoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();

var app = builder.Build();

//
// ---------- SAFE MIGRATION ----------
//
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database migration failed");
    }
}

//
// ---------- MIDDLEWARE ----------
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
