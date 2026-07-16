using Microsoft.EntityFrameworkCore;
using SmarketApi.Models;

namespace SmarketApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tes tables réelles de SMarketdb
        //EF Core → simplifie l’accès à la base (mapping, LINQ, CRUD, migrations).

        //Sécurité serveur → protège ton API avec authentification, autorisation, validation, HTTPS.
        public DbSet<TblUser> TblUser { get; set; } = null!;
        public DbSet<TblCategory> TblCategory { get; set; } = null!;
        public DbSet<TblClient> TblClient { get; set; } = null!;                                             
        public DbSet<TblDetailSelling> TblDetailSelling { get; set; } = null!;
        public DbSet<TblProduct> TblProduct { get; set; } = null!;
        public DbSet<TblSeller> TblSeller { get; set; } = null!;
        public DbSet<TblSelling> TblSelling { get; set; } = null!;
        public DbSet<TblStock> TblStock { get; set; } = null!;
        public DbSet<TblStockMouvement> TblStockMouvement { get; set; } = null!;
    }
}
