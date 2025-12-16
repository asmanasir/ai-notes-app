using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Data;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

// 🔴 REQUIRED FOR AZURE (App Service listens on 8080 internally)
builder.WebHost.UseUrls("http://*:8080");

// --------------------
// SERVICES
// --------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=notes.db"
    )
);

builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();

// ✅ CORS (THIS IS THE KEY FIX)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Needed for Azure reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// --------------------
// MIDDLEWARE (ORDER MATTERS)
// --------------------

app.UseForwardedHeaders();

// ✅ Swagger (safe before CORS)
app.UseSwagger();
app.UseSwaggerUI();

// ❗❗ CORS MUST BE BEFORE MapControllers
app.UseCors("AllowAll");

// Optional but safe
app.UseHttpsRedirection();

app.UseAuthorization();

// --------------------
// ENDPOINTS
// --------------------

app.MapControllers();

app.Run();
