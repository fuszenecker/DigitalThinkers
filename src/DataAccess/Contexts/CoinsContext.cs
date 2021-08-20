using Microsoft.EntityFrameworkCore;
using DigitalThinkers.DataAccess.Entities;

namespace DigitalThinkers.DataAccess.Contexts
{
    public class CoinsContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=notes.db");

        public DbSet<CoinCount> Coins { get; set; }
    }
}