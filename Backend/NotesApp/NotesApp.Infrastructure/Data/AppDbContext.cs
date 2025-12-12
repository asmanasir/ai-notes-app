using Microsoft.EntityFrameworkCore;
using NotesApp.Domain.Entities;
using System.Collections.Generic;

namespace NotesApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notes> Notes { get; set; }
    }
}
