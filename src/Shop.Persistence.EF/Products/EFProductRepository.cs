using Shop.Entities;
using Shop.Services.Products.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _dataContext.Products.FirstOrDefault(_ => _.Code == code).InStockCount += count;
        }

        public void Delete(Product product)
        {
            _dataContext.Remove(product);
        }

        public Product FindByCode(int code)
        {
            return _dataContext.Products.Where(_ => _.Code == code).FirstOrDefault();
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
    }
}
