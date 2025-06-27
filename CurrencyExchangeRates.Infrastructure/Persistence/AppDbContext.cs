using CurrencyExchangeRates.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<CurrencyRate> CurrencyRates { get; set; } = null!;
        public DbSet<Domain.Entities.Wallet> Wallets { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyRate>()
                .HasIndex(x => new { x.CurrencyCode, x.Date })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
