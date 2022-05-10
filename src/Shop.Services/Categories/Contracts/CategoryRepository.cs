using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using System.Collections.Generic;

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
