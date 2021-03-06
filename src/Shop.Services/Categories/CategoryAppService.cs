using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Categories.Contracts;
using Shop.Services.Categories.Exceptions;
using Shop.Services.Products.Contracts;
using System.Collections.Generic;

namespace Shop.Services.Categories
{
    public class CategoryAppService : CategoryService
    {
        private readonly CategoryRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        public CategoryAppService(CategoryRepository repository, 
            UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void Add(AddCategoryDto dto)
        {
            var isCategoryExist = _repository.IsExistCategoryTitle(dto.Title);

            if (isCategoryExist)
            {
                throw new DuplicateCategoryTitleException();
            }

            var category = new Category
            {
                Title = dto.Title
            };

            _repository.Add(category);
            _unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            var category = _repository.FindById(id);

            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            int productCount = _repository.CategoryProductCount(id);

            if (productCount > 0)
            {
                throw new UnableToDeleteCategoryWithExistingProductException();
            }

            _repository.Delete(category);
            _unitOfWork.Commit();
        }

        public IList<GetCategoryDto> GetAll()
        {
            return _repository.GetAll();
        }

        public IList<GetProductDto> GetProducts(int id)
        {
            return _repository.GetProducts(id);
        }

        public void Update(int id, UpdateCategoryDto dto)
        {
            Category category = _repository.FindById(id);

            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            category.Title = dto.Title;
            _unitOfWork.Commit();
        }
    }
}
