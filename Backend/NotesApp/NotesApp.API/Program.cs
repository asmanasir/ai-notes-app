using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.ApplicationInsights;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

//
// ---------- LOGGING (IMPORTANT) ----------
//
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ✅ Enable ILogger → Application Insights
builder.Logging.AddApplicationInsights();
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
    "",
    LogLevel.Information
);

//
// ---------- CORS ----------
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

//
// ---------- APPLICATION INSIGHTS ----------
//
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.EnableAdaptiveSampling = false; // good for debugging
});

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
// ---------- APPLICATION LAYER ----------
//
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<INoteRepository, SqlNoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddSingleton<TelemetryClient>();

//
// ---------- BUILD ----------
//
var app = builder.Build();

//
// ---------- SAFE MIGRATIONS ----------
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
        app.Logger.LogError(ex, "❌ Database migration failed");
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
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
