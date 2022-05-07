using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Products.Contracts
{
    public interface ProductService : Service
    {
        void Add(DefineProductDto dto);
        void AddToStock(AddProductToStockDto dto);
        void Delete(int code);
        void Update(int code, UpdateProductDto dto);
        IList<GetProductDto> GetAll();
        GetProductWithPurchaseBillsDto GetPurchaseBills(int code);
        GetProductWithSaleBillsDto GetSaleBills(int code);
    }
}
