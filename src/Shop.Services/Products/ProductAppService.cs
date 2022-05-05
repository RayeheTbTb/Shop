using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Products
{
    public class ProductAppService : ProductService
    {
        private readonly ProductRepository _repository;
        private readonly UnitOfWork _unitOfWork;

        public ProductAppService(ProductRepository repository, UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void Add(DefineProductDto dto)
        {
            var isCodeExist = _repository.IsExistCode(dto.Code);
            if (isCodeExist)
            {
                throw new DuplicateProductCodeException();
            }

            var isNameExistInCategory = _repository.IsExistNameInCategory(dto.CategoryId, dto.Name);

            if (isNameExistInCategory)
            {
                throw new DuplicateProductNameInCategoryException();
            }

            var product = new Product
            {
                Name = dto.Name,
                Code = dto.Code,
                MinimumInStock = dto.MinimumInStock,
                CategoryId = dto.CategoryId
            };
            _repository.Add(product);
            _unitOfWork.Commit();
        }
    }
}
