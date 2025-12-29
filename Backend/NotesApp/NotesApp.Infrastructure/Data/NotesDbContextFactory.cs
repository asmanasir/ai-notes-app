using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NotesApp.Infrastructure.Data;

namespace NotesApp.Infrastructure.Data
{
    public class NotesDbContextFactory
        : IDesignTimeDbContextFactory<NotesDbContext>
    {
        public NotesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();

            // 🔴 IMPORTANT: use the SAME connection string you used earlier
            optionsBuilder.UseSqlServer(
                "Server=tcp:notesapp-sql-server.database.windows.net,1433;" +
                "Database=NotesAppDb;" +
                "User ID=notesadmin;" +
                "Password=Sikkerhet1234;" +
                "Encrypt=True;" +
                "TrustServerCertificate=False;"
            );

            return new NotesDbContext(optionsBuilder.Options);
        }
    }
}
