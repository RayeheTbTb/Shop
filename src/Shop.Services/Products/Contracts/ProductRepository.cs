using Shop.Entities;
using Shop.Infrastructure.Application;
using System.Collections.Generic;

namespace Shop.Services.Products.Contracts
{
    public interface ProductRepository : Repository
    {
        void Add(Product product);
        bool IsExistCode(int code);
        bool IsExistNameInCategory(int categoryId, string name);
        bool IsNameDuplicate(string name, int code);
        Product FindByCode(int code);
        void AddtoStock(int id, int count);
        void Delete(Product product);
        IList<GetProductDto> GetAll();
        GetProductWithPurchaseBillsDto GetPurchaseBills(int code);
        GetProductWithSaleBillsDto GetSaleBills(int code);
        Product FindById(int productId);
        void RemoveFromStock(int id, int count);
        GetProductDto Get(int code);
    }
}
