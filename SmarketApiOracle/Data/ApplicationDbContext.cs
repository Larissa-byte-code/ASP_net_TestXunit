using Microsoft.EntityFrameworkCore;
using SmarketApi.Models;

namespace SmarketApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<TblUser> TblUser { get; set; }
        public DbSet<TblCategory> TblCategory { get; set; }
        public DbSet<TblClient> TblClient { get; set; }
        public DbSet<TblProduct> TblProduct { get; set; }
        public DbSet<TblSeller> TblSeller { get; set; }
        public DbSet<TblSelling> TblSelling { get; set; }
        public DbSet<TblDetailSelling> TblDetailSelling { get; set; }
        public DbSet<TblStock> TblStock { get; set; }
        public DbSet<TblStockMovement> TblStockMovement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapping explicite si nécessaire
            modelBuilder.Entity<TblUser>().ToTable("TblUser");
            modelBuilder.Entity<TblCategory>().ToTable("TblCategory");
            modelBuilder.Entity<TblClient>().ToTable("TblClient");
            modelBuilder.Entity<TblProduct>().ToTable("TblProduct");
            modelBuilder.Entity<TblSeller>().ToTable("TblSeller");
            modelBuilder.Entity<TblSelling>().ToTable("TblSelling");
            modelBuilder.Entity<TblDetailSelling>().ToTable("TblDetailSelling");
            modelBuilder.Entity<TblStock>().ToTable("TblStock");
            modelBuilder.Entity<TblStockMovement>().ToTable("TblStockMovement");

            // Exemple pour Oracle : préciser les colonnes décimales
            modelBuilder.Entity<TblDetailSelling>()
                .Property(d => d.Price)
                .HasColumnType("NUMBER(18,2)");

            modelBuilder.Entity<TblDetailSelling>()
                .Property(d => d.LineTotal)
                .HasColumnType("NUMBER(18,2)");

            modelBuilder.Entity<TblSelling>()
                .Property(s => s.TotalAmount)
                .HasColumnType("NUMBER(18,2)");
        }
    }
}
