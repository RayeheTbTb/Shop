using Shop.Entities;
using Shop.Services.Categories.Contracts;
using Shop.Services.Products.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Persistence.EF.Categories
{
    public class EFCategoryRepository : CategoryRepository
    {
        private readonly EFDataContext _dataContext;

        public EFCategoryRepository(EFDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(Category category)
        {
            _dataContext.Categories.Add(category);
        }

        public int CategoryProductCount(int id)
        {
            return _dataContext.Categories
                .FirstOrDefault(_ => _.Id == id).Products.Count();
        }

        public void Delete(Category category)
        {
            _dataContext.Categories.Remove(category);
        }

        public Category FindById(int id)
        {
            return _dataContext.Categories.Find(id);
        }

        public IList<GetCategoryDto> GetAll()
        {
            return _dataContext.Categories
                .Select(_ => new GetCategoryDto
                {
                    Id = _.Id,
                    Title = _.Title
                }).ToList();
        }

        public IList<GetProductDto> GetProducts(int id)
        {
            return _dataContext.Categories
                .FirstOrDefault(_ => _.Id == id)
                .Products.Select(_ => new GetProductDto
                {
                    Id = _.Id,
                    Code = _.Code,
                    Name = _.Name,
                    InStockCount = _.InStockCount,
                    Price = _.Price
                }).ToList();
        }

        public bool IsExistCategoryTitle(string title)
        {
            return _dataContext.Categories.Any(_ => _.Title == title);
        }
    }
}
