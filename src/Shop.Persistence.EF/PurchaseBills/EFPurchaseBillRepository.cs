using Shop.Entities;
using Shop.Services.PurchaseBills.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Persistence.EF.PurchaseBills
{
    public class EFPurchaseBillRepository : PurchaseBillRepository
    {
        private readonly EFDataContext _dataContext;

        public EFPurchaseBillRepository(EFDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(PurchaseBill purchaseBill)
        {
            _dataContext.PurchaseBills.Add(purchaseBill);
        }

        public bool BillExistsForProduct(int code)
        {
            return _dataContext.PurchaseBills.Any(_ => _.Product.Code == code);
        }

        public PurchaseBill FindById(int id)
        {
            return _dataContext.PurchaseBills.Find(id);
        }
    }
}
