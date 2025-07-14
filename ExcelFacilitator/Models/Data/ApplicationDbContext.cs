using ExcelFacilitator.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExcelFacilitator.Models.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<ExcelRecord> ExcelRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.SellerTin);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.BuyerTin);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.ItemCatalogCode);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.SourceFile);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.ContractDate);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.DocumentDate);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.Quantity);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.Price);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.PriceWithVat);

            modelBuilder.Entity<ExcelRecord>()
                .HasIndex(e => e.TotalWithVat);
        }
    }
}
