using Microsoft.EntityFrameworkCore;
using NotesApp.Infrastructure.Data;
using NotesApp.Application.Interfaces;
using NotesApp.Infrastructure.Repositories;
using NotesApp.Application.Services;
using NotesApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------- SERVICES --------------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=notes.db" // fallback for safety
    )
);

builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IAiService, AiService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// -------------------- APP --------------------

var app = builder.Build();

app.UseCors("AllowAll");

// 🔥 Swagger ENABLED for Azure (important)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
