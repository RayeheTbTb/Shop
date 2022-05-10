using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Persistence.EF.Categories;

namespace Shop.Persistence.EF
{
    public class EFDataContext : DbContext
    {
        
        public EFDataContext(string connectionString) :
            this(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        { }

        public EFDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly
                (typeof(CategoryEntityMap).Assembly);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseBill> PurchaseBills { get; set; }
        public DbSet<SaleBill> SaleBills { get; set; }
    }
}
