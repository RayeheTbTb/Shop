using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.PurchaseBills.Exceptions;
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
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;

        public ProductAppService(ProductRepository repository, UnitOfWork unitOfWork, PurchaseBillRepository purchaseBillRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _purchaseBillRepository = purchaseBillRepository;
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

        public void AddToStock(AddProductToStockDto dto)
        {
            Product product = _repository.FindByCode(dto.Code);
            if (product == null)
            {
                throw new ProductDoesNotExistException();
            }

            var purchaseBill = new PurchaseBill
            {
                SellerName = dto.SellerName,
                Count = dto.Count,
                Date = dto.Date,
                WholePrice = dto.WholePrice,
                Product = product,
                ProductId = product.Id
            };

            _repository.AddtoStock(dto.Code, dto.Count);
            _purchaseBillRepository.Add(purchaseBill);
            _unitOfWork.Commit();
        }

        public void Delete(int code)
        {
            Product product = _repository.FindByCode(code);
            bool billForProductExists = _purchaseBillRepository.BillExistsForProduct(code);

            if (billForProductExists)
            {
                throw new UnableToDeleteProductWithExistingPurchaseBillException();
            }

            if(product == null)
            {
                throw new ProductDoesNotExistException();
            }

            _repository.Delete(product);
            _unitOfWork.Commit();
        }

        public void Update(int code, UpdateProductDto dto)
        {
            var product = _repository.FindByCode(code);
            if (product == null)
            {
                throw new ProductDoesNotExistException();
            }

            var isNameDuplicate = _repository.IsExistNameInCategory(product.CategoryId, dto.Name);
            if (isNameDuplicate)
            {
                throw new DuplicateProductNameInCategoryException();
            }

            product.Name = dto.Name;
            product.MinimumInStock = dto.MinimumInStock;

            _unitOfWork.Commit();
        }
    }
}
