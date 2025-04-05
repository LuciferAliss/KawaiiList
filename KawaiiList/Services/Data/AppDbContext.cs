using KawaiiList.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.Services.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=anilibria_db;Username=postgres;Password=yourpassword");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка индексов и отношений
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<AuthToken>()
                .HasIndex(t => t.Token)
                .IsUnique();
        }
    }
}
