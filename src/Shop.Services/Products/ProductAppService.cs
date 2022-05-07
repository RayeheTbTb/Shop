using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.PurchaseBills.Exceptions;
using Shop.Services.SaleBills.Exceptions;
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
                throw new ProductNotFoundException();
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
                throw new ProductNotFoundException();
            }

            _repository.Delete(product);
            _unitOfWork.Commit();
        }
        
        public IList<GetProductDto> GetAll()
        {
            return _repository.GetAll();
        }

        public GetProductWithPurchaseBillsDto GetPurchaseBills(int code)
        {
            var product = _repository.FindByCode(code);
            if (product == null)
            {
                throw new ProductNotFoundException();
            }
            if (product.PurchaseBills.Count() == 0)
            {
                throw new NoPurchaseBillsExistException();
            }
            return _repository.GetPurchaseBills(code);
        }

        public GetProductWithSaleBillsDto GetSaleBills(int code)
        {
            var product = _repository.FindByCode(code);
            if (product == null)
            {
                throw new ProductNotFoundException();
            }
            if (product.SaleBills.Count() == 0)
            {
                throw new NoSaleBillsExistException();
            }

            return _repository.GetSaleBills(code);
        }

        public void Update(int code, UpdateProductDto dto)
        {
            var product = _repository.FindByCode(code);
            if (product == null)
            {
                throw new ProductNotFoundException();
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
