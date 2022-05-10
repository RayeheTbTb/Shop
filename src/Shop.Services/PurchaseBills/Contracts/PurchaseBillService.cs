using Shop.Infrastructure.Application;
using System.Collections.Generic;

namespace Shop.Services.PurchaseBills.Contracts
{
    public interface PurchaseBillService : Service
    {
        void Update(int id, UpdatePurchaseBillDto dto);
        void Delete(int billId);
        IList<GetPurchaseBillDto> GetAll();
        GetPurchaseBillDto Get(int id);
        void Add(AddPurchaseBillDto dto);
    }
}
