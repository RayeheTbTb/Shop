using Shop.Entities;
using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.PurchaseBills.Contracts
{
    public interface PurchaseBillRepository : Repository
    {
        void Add(PurchaseBill purchaseBill);
    }
}
