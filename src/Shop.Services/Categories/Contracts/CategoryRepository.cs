using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Categories.Contracts
{
    public interface CategoryRepository : Repository
    {
        void Add(Category category);
        bool IsExistCategoryTitle(string title);
        Category FindById(int id);
        void Delete(Category category);
        int CategoryProductCount(int id);
        IList<GetCategoryDto> GetAll();
        IList<GetProductDto> GetProducts(int id);
    }
}
