using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Data
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
        public DbSet<TblStockMouvement> TblStockMouvement { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    // Mapping explicite des tables
                    modelBuilder.Entity<TblUser>().ToTable("TBLUSER");
                    modelBuilder.Entity<TblCategory>().ToTable("TBLCATEGORY");
                    modelBuilder.Entity<TblClient>().ToTable("TBLCLIENT");
                    modelBuilder.Entity<TblProduct>().ToTable("TBLPRODUCT");
                    modelBuilder.Entity<TblSeller>().ToTable("TBLSELLER");
                    modelBuilder.Entity<TblSelling>().ToTable("TBLSELLING");
                    modelBuilder.Entity<TblDetailSelling>().ToTable("TBLDETAILSELLING");
                    modelBuilder.Entity<TblStock>().ToTable("TBLSTOCK");
                    modelBuilder.Entity<TblStockMouvement>().ToTable("TBLSTOCKMOUVEMENT");

                    // Colonnes décimales pour Oracle
                    modelBuilder.Entity<TblDetailSelling>()
                        .Property(d => d.Price)
                        .HasColumnType("NUMBER(18,2)");

                    modelBuilder.Entity<TblDetailSelling>()
                        .Property(d => d.LineTotal)
                        .HasColumnType("NUMBER(18,2)");

                    modelBuilder.Entity<TblSelling>()
                        .Property(s => s.TotalAmount)
                        .HasColumnType("NUMBER(18,2)");

                    // Correction des booléens pour Oracle
                    modelBuilder.Entity<TblClient>()
                        .Property(c => c.IsActive)
                        .HasConversion<int>()       // convertit true/false en 1/0
                        .HasColumnType("NUMBER(1)");

                    // TblCategory
                    modelBuilder.Entity<TblCategory>(entity =>
                    {
                        entity.HasKey(e => e.CatId);
                        entity.Property(e => e.CatId).HasColumnName("CATID");
                        entity.Property(e => e.CatName).HasColumnName("CATNAME");
                        entity.Property(e => e.CatDes).HasColumnName("CATDES");
                        entity.Property(e => e.CatIdvC).HasColumnName("CATIDVC");
                    });

                    // TblClient
                    modelBuilder.Entity<TblClient>(entity =>
                    {
                        entity.HasKey(e => e.ClientId);
                        entity.Property(e => e.ClientId).HasColumnName("CLIENTID");
                        entity.Property(e => e.ClientCode).HasColumnName("CLIENTCODE");
                        entity.Property(e => e.ClientName).HasColumnName("CLIENTNAME");
                        entity.Property(e => e.ClientPhone).HasColumnName("CLIENTPHONE");
                        entity.Property(e => e.ClientEmail).HasColumnName("CLIENTEMAIL");
                        entity.Property(e => e.ClientAddress).HasColumnName("CLIENTADDRESS");
                        entity.Property(e => e.DateCreated).HasColumnName("DATECREATED");
                        entity.Property(e => e.IsActive).HasColumnName("ISACTIVE");
                    });

                    // TblProduct
                    modelBuilder.Entity<TblProduct>(entity =>
                    {
                        entity.HasKey(e => e.prdId);
                        entity.Property(e => e.prdId).HasColumnName("PRDID");
                        entity.Property(e => e.prdName).HasColumnName("PRDNAME");
                        entity.Property(e => e.prdCat).HasColumnName("PRDCAT");
                        entity.Property(e => e.prdIdvC).HasColumnName("PRDIDVC");
                    });

                    // TblSeller
                    modelBuilder.Entity<TblSeller>(entity =>
                    {
                        entity.HasKey(e => e.SellerId);
                        entity.Property(e => e.SellerId).HasColumnName("SELLERID");
                        entity.Property(e => e.SellerName).HasColumnName("SELLERNAME");
                        entity.Property(e => e.SellerAge).HasColumnName("SELLERAGE");
                        entity.Property(e => e.SellerPhone).HasColumnName("SELLERPHONE");
                        entity.Property(e => e.SellerPass).HasColumnName("SELLERPASS");
                        entity.Property(e => e.SellerIdvC).HasColumnName("SELLERIDVC");
                        entity.Property(e => e.Role).HasColumnName("ROLE");
                    });

                    // TblSelling
                    modelBuilder.Entity<TblSelling>(entity =>
                    {
                        entity.HasKey(e => e.VenteId);
                        entity.Property(e => e.VenteId).HasColumnName("VENTEID");
                        entity.Property(e => e.DateVente).HasColumnName("DATEVENTE");
                        entity.Property(e => e.SellerName).HasColumnName("SELLERNAME");
                        entity.Property(e => e.ClientName).HasColumnName("CLIENTNAME");
                        entity.Property(e => e.Numfacture).HasColumnName("NUMFACTURE");
                        entity.Property(e => e.ModeDePaiement).HasColumnName("MODEDEPAIEMENT");
                        entity.Property(e => e.TotalAmount)
                            .HasColumnName("TOTALAMOUNT")
                            .HasColumnType("NUMBER(18,2)");
                    });

                    // TblDetailSelling
                    modelBuilder.Entity<TblDetailSelling>(entity =>
                    {
                        entity.HasKey(e => e.DetailId);
                        entity.Property(e => e.DetailId).HasColumnName("DETAILID");
                        entity.Property(e => e.VenteId).HasColumnName("VENTEID");
                        entity.Property(e => e.ProductId).HasColumnName("PRODUCTID"); 
                        entity.Property(e => e.ProductName).HasColumnName("PRODUCTNAME");
                        entity.Property(e => e.Qty).HasColumnName("QTY");
                        entity.Property(e => e.Price).HasColumnName("PRICE").HasColumnType("NUMBER(18,2)");
                        entity.Property(e => e.LineTotal).HasColumnName("LINETOTAL").HasColumnType("NUMBER(18,2)");
                    });

                    // TblStock
                    modelBuilder.Entity<TblStock>(entity =>
                    {
                        entity.HasKey(e => e.StockId);
                        entity.Property(e => e.StockId).HasColumnName("STOCKID");
                        entity.Property(e => e.ProductId).HasColumnName("PRODUCTID"); 
                        entity.Property(e => e.QtyDisponible).HasColumnName("QTYDISPONIBLE");
                        entity.Property(e => e.DateMaj).HasColumnName("DATEMAJ");
                        entity.Property(e => e.CatId).HasColumnName("CATID");
                        entity.Property(e => e.CodeFormate).HasColumnName("CODEFORMATE");
                    });
                    //
                    modelBuilder.Entity<TblStockMouvement>(entity =>
                        {
                            entity.HasKey(e => e.MouvementId);

                            entity.Property(e => e.MouvementId).HasColumnName("MOUVEMENTID");
                            entity.Property(e => e.ProductId).HasColumnName("PRODUCTID");
                            entity.Property(e => e.TypeMouvement).HasColumnName("TYPEMOUVEMENT"); 
                            entity.Property(e => e.Qty).HasColumnName("QTY");
                            entity.Property(e => e.DateMouvement).HasColumnName("DATEMOUVEMENT");
                            entity.Property(e => e.RefVenteId).HasColumnName("REFVENTEID");
                            entity.Property(e => e.Commentaire).HasColumnName("COMMENTAIRE");
                        });

                }

    }
}
