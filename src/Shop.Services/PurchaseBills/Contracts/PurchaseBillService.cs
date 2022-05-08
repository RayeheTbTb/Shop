using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.PurchaseBills.Contracts
{
    public interface PurchaseBillService : Service
    {
        void Update(int id, UpdatePurchaseBillDto dto);
        void Delete(int billId);
        IList<GetPurchaseBillDto> GetAll();
        GetPurchaseBillDto Get(int id);
    }
}
