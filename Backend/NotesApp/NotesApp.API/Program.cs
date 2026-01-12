using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

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
    options.EnableAdaptiveSampling = false;
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
                5,
                TimeSpan.FromSeconds(30),
                null);
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
// ---------- DB MIGRATION ----------
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

app.UseHttpsRedirection();

// ✅ CORS MUST be before MapControllers
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
