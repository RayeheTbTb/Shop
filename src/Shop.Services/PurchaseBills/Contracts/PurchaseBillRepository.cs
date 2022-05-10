using Shop.Entities;
using Shop.Infrastructure.Application;
using System.Collections.Generic;

namespace Shop.Services.PurchaseBills.Contracts
{
    public interface PurchaseBillRepository : Repository
    {
        void Add(PurchaseBill purchaseBill);
        bool BillExistsForProduct(int code);
        PurchaseBill FindById(int id);
        void Delete(PurchaseBill purchaseBill);
        IList<GetPurchaseBillDto> GetAll();
        GetPurchaseBillDto Get(int id);
        bool IsExistPurchaseBill(int id);
    }
}
