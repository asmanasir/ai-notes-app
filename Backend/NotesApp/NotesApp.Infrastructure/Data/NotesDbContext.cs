using Microsoft.EntityFrameworkCore;
using NotesApp.Domain.Entities;

namespace NotesApp.Infrastructure.Data
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; } = null!;

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

                entity.Property(n => n.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}