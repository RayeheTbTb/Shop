using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Categories.Contracts
{
    public interface CategoryService : Service
    {
        void Add(AddCategoryDto dto);
        void Update(int id, UpdateCategoryDto dto);
        void Delete(int id);
        IList<GetCategoryDto> GetAll();
        IList<GetProductDto> GetProducts(int id);
    }
}
