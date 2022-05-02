using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Persistence.EF.Items
{
    public class ItemEntityMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> _)
        {
            _.ToTable("Products");

            _.HasKey(_ => _.Id);
            _.Property(_ => _.Id)
                .ValueGeneratedOnAdd();

            _.HasMany(_ => _.SaleBills)
                .WithOne(_ => _.Product)
                .HasForeignKey(_ => _.ProductId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            _.HasMany(_ => _.PurchaseBills)
                .WithOne(_ => _.Product)
                .HasForeignKey(_ => _.ProductId)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
