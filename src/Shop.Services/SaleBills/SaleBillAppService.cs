using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using Shop.Services.SaleBills.Contracts;
using Shop.Services.SaleBills.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
