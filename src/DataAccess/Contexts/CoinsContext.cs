using Microsoft.EntityFrameworkCore;
using ServiceTemplate.DataAccess.Entities;

namespace ServiceTemplate.DataAccess.Contexts
{
    public class CoinsContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=coins.db");

        public DbSet<CoinCount> Coins { get; set; }
    }
}