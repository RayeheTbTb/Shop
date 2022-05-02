using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Persistence.EF.SaleBills
{
    public class SaleBillEntityMap : IEntityTypeConfiguration<SaleBill>
    {
        public void Configure(EntityTypeBuilder<SaleBill> _)
        {
            _.ToTable("SaleBills");

            _.HasKey(_ => _.Id);
            _.Property(_ => _.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
