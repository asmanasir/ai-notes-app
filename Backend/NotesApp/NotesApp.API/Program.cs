using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//
// ---------- CORS (ONCE ONLY) ----------
// Local + Azure Static Web App
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
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.EnableAdaptiveSampling = false;
});

//
// ---------- SWAGGER ----------
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// ---------- DATABASE (SQL SERVER) ----------
// Uses:
// - User Secrets (local)
// - Azure App Service → Connection Strings (prod)
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

//
// ---------- BUILD ----------
//
var app = builder.Build();

//
// ---------- SAFE DATABASE MIGRATION ----------
// Does NOT crash app if DB is temporarily unavailable
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
// ---------- MIDDLEWARE (ORDER MATTERS) ----------
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔴 CORS MUST BE BEFORE MapControllers
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var traceId = context.TraceIdentifier;

    using (context.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("RequestScope")
        .BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = traceId
        }))
    {
        await next();
    }
});

app.MapHealthChecks("/health");
app.MapControllers();
app.Run();
