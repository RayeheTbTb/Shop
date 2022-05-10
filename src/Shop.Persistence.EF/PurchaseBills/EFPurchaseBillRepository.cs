using Shop.Entities;
using Shop.Services.PurchaseBills.Contracts;
using System.Collections.Generic;
using System.Linq;

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

        public void Delete(PurchaseBill purchaseBill)
        {
            _dataContext.PurchaseBills.Remove(purchaseBill);
        }

        public PurchaseBill FindById(int id)
        {
            return _dataContext.PurchaseBills.Find(id);
        }

        public GetPurchaseBillDto Get(int id)
        {
            return _dataContext.PurchaseBills.Where(_ => _.Id == id)
                .Select(_ => new GetPurchaseBillDto
                {
                    Count = _.Count,
                    Id = _.Id,
                    WholePrice = _.WholePrice,
                    Date = _.Date,
                    SellerName = _.SellerName,
                    ProductId = _.ProductId
                }).FirstOrDefault();
        }

        public IList<GetPurchaseBillDto> GetAll()
        {
            return _dataContext.PurchaseBills
                .Select(_ => new GetPurchaseBillDto
                {
                    ProductId = _.ProductId,
                    SellerName = _.SellerName,
                    WholePrice = _.WholePrice,
                    Count = _.Count,
                    Date = _.Date,
                    Id = _.Id
                }).ToList();
        }

        public bool IsExistPurchaseBill(int id)
        {
            return _dataContext.PurchaseBills.Any(_ => _.Id == id);
        }
    }
}
