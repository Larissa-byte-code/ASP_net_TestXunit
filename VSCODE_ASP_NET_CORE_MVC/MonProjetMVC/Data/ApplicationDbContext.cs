using Microsoft.EntityFrameworkCore;
using MonProjetMVC.Models;

namespace MonProjetMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tes tables réelles
        public DbSet<TblCategory> TblCategory { get; set; }
        public DbSet<TblClient> TblClient { get; set; }
        public DbSet<TblDetailSelling> TblDetailSelling { get; set; }
        public DbSet<TblProduct> TblProduct { get; set; }
        public DbSet<TblSeller> TblSeller { get; set; }
        public DbSet<TblSelling> TblSelling { get; set; }
        public DbSet<TblStock> TblStock { get; set; }
        public DbSet<TblStockMouvement> TblStockMouvement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapping des tables existantes
            modelBuilder.Entity<TblCategory>().ToTable("TblCategory");
            modelBuilder.Entity<TblClient>().ToTable("TblClient");
            modelBuilder.Entity<TblDetailSelling>().ToTable("TblDetailSelling");
            modelBuilder.Entity<TblProduct>().ToTable("TblProduct");
            modelBuilder.Entity<TblSeller>().ToTable("TblSeller");
            modelBuilder.Entity<TblSelling>().ToTable("TblSelling");
            modelBuilder.Entity<TblStock>().ToTable("TblStock");
            modelBuilder.Entity<TblStockMouvement>().ToTable("TblStockMouvement");

            // Configuration des colonnes décimales pour éviter les warnings
            

            modelBuilder.Entity<TblDetailSelling>()
                .Property(d => d.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TblDetailSelling>()
                .Property(d => d.LineTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TblSelling>()
                .Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
