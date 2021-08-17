using DigitalThinkers.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalThinkers.DataAccess.Contexts
{
    public class NotesContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=notes.db");

        public DbSet<Note> Notes { get; set; }
    }
}