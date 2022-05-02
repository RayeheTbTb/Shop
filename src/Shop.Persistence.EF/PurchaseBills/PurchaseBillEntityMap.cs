using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Persistence.EF.PurchaseBills
{
    public class PurchaseBillEntityMap : IEntityTypeConfiguration<PurchaseBill>
    {
        public void Configure(EntityTypeBuilder<PurchaseBill> _)
        {
            _.ToTable("PurchaseBills");

            _.HasKey(_ => _.Id);
            _.Property(_ => _.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
