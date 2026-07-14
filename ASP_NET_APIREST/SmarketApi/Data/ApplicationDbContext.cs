using Microsoft.EntityFrameworkCore;
using SmarketApi.Models;

namespace SmarketApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tes tables réelles de SMarketdb
    
        public DbSet<TblCategory> TblCategory { get; set; }
        public DbSet<TblClient> TblClient { get; set; }                                              
        public DbSet<TblDetailSelling> TblDetailSelling { get; set; }
        public DbSet<TblProduct> TblProduct { get; set; }
        public DbSet<TblSeller> TblSeller { get; set; }
        public DbSet<TblSelling> TblSelling { get; set; }
        public DbSet<TblStock> TblStock { get; set; }
        public DbSet<TblStockMouvement> TblStockMouvement { get; set; }
    }
}
