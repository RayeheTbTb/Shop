using Shop.Entities;
using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Products.Contracts
{
    public interface ProductRepository : Repository
    {
        void Add(Product product);
        bool IsExistCode(int code);
        bool IsExistNameInCategory(int categoryId, string name);
        Product FindByCode(int code);
        void AddtoStock(int code, int count);
    }
}
