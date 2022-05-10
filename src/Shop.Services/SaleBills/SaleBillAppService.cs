using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using Shop.Services.SaleBills.Contracts;
using Shop.Services.SaleBills.Exceptions;
using System.Collections.Generic;

namespace Shop.Services.SaleBills
{
    public class SaleBillAppService : SaleBillService
    {
        private readonly SaleBillRepository _repository;
        private readonly ProductRepository _productRepository;
        private readonly UnitOfWork _unitOfWork;

        public SaleBillAppService(SaleBillRepository repository,
            UnitOfWork unitOfWork, ProductRepository productRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public void Add(AddSaleBillDto dto)
        {
            var product = _productRepository.FindByCode(dto.ProductCode);
            if (product == null)
            {
                throw new ProductNotFoundException();
            }
            if (dto.Count > product.InStockCount)
            {
                throw new NotEnoughProductInStockException();
            }

            var saleBill = new SaleBill
            {
                CustomerName = dto.CustomerName,
                Date = dto.Date,
                WholePrice = dto.WholePrice,
                Count = dto.Count,
                ProductId = product.Id,
                Product = product
            };
            _repository.Add(saleBill);
            _productRepository.RemoveFromStock(product, dto.Count);

            if(product.InStockCount == 0)
            {
                throw new ProductOutOfStockException();
            }

            if (product.InStockCount <= product.MinimumInStock)
            {
                throw new ProductReachedMinimumInStockCountException();
            }

            _unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            SaleBill saleBill = _repository.FindById(id);
            if(saleBill == null)
            {
                throw new SaleBillNotFoundException();
            }

            _productRepository.AddtoStock(saleBill.Product.Code, saleBill.Count);
            _repository.Delete(saleBill);
            _unitOfWork.Commit();
        }

        public GetSaleBillDto Get(int id)
        {
            bool isBillExist = _repository.IsExistBill(id);
            if (!isBillExist)
            {
                throw new SaleBillNotFoundException();
            }
            return _repository.Get(id);
        }

        public IList<GetSaleBillDto> GetAll()
        {
            return _repository.GetAll();
        }

        public void Update(int id, UpdateSaleBillDto dto)
        {
            var product = _productRepository.FindByCode(dto.ProductCode);
            var saleBill = _repository.FindById(id);
            if(saleBill == null)
            {
                throw new SaleBillNotFoundException();
            }

            Product previousProduct = _productRepository
                .FindById(saleBill.ProductId);
            if(product.InStockCount < dto.Count)
            {
                throw new NotEnoughProductInStockException();
            }

            _productRepository.AddtoStock(previousProduct.Code, dto.Count);

            saleBill.Product = product;
            saleBill.ProductId = product.Id;
            saleBill.Count = dto.Count;
            saleBill.CustomerName = dto.CustomerName;
            saleBill.WholePrice = dto.WholePrice;
            _productRepository.RemoveFromStock(product, saleBill.Count);

            if (product.InStockCount == 0)
            {
                throw new ProductOutOfStockException();
            }

            if (product.InStockCount <= product.MinimumInStock)
            {
                throw new ProductReachedMinimumInStockCountException();
            }

            _unitOfWork.Commit();
        }
    }
}
