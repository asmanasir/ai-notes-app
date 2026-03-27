using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotesApp.Domain.Entities;
using System.Text.Json;

namespace NotesApp.Infrastructure.Data
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("Notes"); // ✅ Explicit table name

                entity.HasKey(n => n.Id);

                entity.Property(n => n.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(n => n.Content)
                      .IsRequired();

                entity.Property(n => n.UserId)
                      .IsRequired();

                entity.Property(n => n.Summary);

                entity.Property(n => n.Pinned)
                      .HasDefaultValue(false);

                entity.Property(n => n.Tags)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                          v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>(),
                          new ValueComparer<List<string>>(
                              (a, b) => a != null && b != null && a.SequenceEqual(b),
                              v => v.Aggregate(0, (a, s) => HashCode.Combine(a, s.GetHashCode())),
                              v => v.ToList()));

                entity.Property(n => n.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}