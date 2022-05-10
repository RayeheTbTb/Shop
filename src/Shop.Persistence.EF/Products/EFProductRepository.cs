using Shop.Entities;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.SaleBills.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Persistence.EF.Products
{
    public class EFProductRepository : ProductRepository
    {
        private readonly EFDataContext _dataContext;

        public EFProductRepository(EFDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(Product product)
        {
            _dataContext.Products.Add(product);
        }

        public void AddtoStock(int code, int count)
        {
            _dataContext.Products
                .FirstOrDefault(_ => _.Code == code).InStockCount += count;
        }

        public void Delete(Product product)
        {
            _dataContext.Remove(product);
        }

        public Product FindByCode(int code)
        {
            return _dataContext.Products
                .Where(_ => _.Code == code).FirstOrDefault();
        }

        public Product FindById(int productId)
        {
            return _dataContext.Products.Find(productId);
        }

        public IList<GetProductDto> GetAll()
        {
            return _dataContext.Products.Select(_ => new GetProductDto
            {
                Name = _.Name,
                Price = _.Price,
                InStockCount = _.InStockCount,
                Code = _.Code,
                Id = _.Id
            }).ToList();
        }

        public GetProductWithPurchaseBillsDto GetPurchaseBills(int code)
        {
            return _dataContext.Products.Where(_ => _.Code == code)
                .Select(_ => new GetProductWithPurchaseBillsDto
                {
                    Code = _.Code,
                    Name = _.Name,
                    InStockCount = _.InStockCount,
                    Price = _.Price,
                    Id = _.Id,
                    PurchaseBills = _.PurchaseBills.Select(_ => new GetPurchaseBillDto
                    {
                        Count = _.Count,
                        SellerName = _.SellerName,
                        WholePrice = _.WholePrice,
                        Date = _.Date,
                        Id = _.Id,
                        ProductId = _.ProductId
                    }).ToList()
                }).FirstOrDefault();
        }

        public GetProductWithSaleBillsDto GetSaleBills(int code)
        {
            return _dataContext.Products.Where(_ => _.Code == code)
                .Select(_ => new GetProductWithSaleBillsDto
                {
                    Code = _.Code,
                    Name = _.Name,
                    InStockCount = _.InStockCount,
                    Price = _.Price,
                    Id = _.Id,
                    SaleBills = _.SaleBills.Select(_ => new GetSaleBillDto
                    {
                        Count = _.Count,
                        CustomerName = _.CustomerName,
                        WholePrice = _.WholePrice,
                        Date = _.Date,
                        Id = _.Id,
                        ProductId = _.ProductId
                    }).ToList()
                }).FirstOrDefault();
        }

        public bool IsExistCode(int code)
        {
            return _dataContext.Products.Any(_ => _.Code == code);
        }

        public bool IsExistNameInCategory(int categoryId, string name)
        {
            return _dataContext.Products
                .Where(_ => _.CategoryId == categoryId && _.Name == name)
                .Any();
        }

        public void RemoveFromStock(Product product, int count)
        {
            product.InStockCount -= count;
        }
    }
}
