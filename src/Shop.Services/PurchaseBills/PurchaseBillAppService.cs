using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.PurchaseBills.Exceptions;
using System.Collections.Generic;

namespace Shop.Services.PurchaseBills
{
    public class PurchaseBillAppService : PurchaseBillService
    {
        private readonly PurchaseBillRepository _repository;
        private readonly ProductRepository _productRepository;
        private readonly UnitOfWork _unitOfWork;

        public PurchaseBillAppService(PurchaseBillRepository repository, 
            UnitOfWork unitOfWork, ProductRepository productRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public void Add(AddPurchaseBillDto dto)
        {
            var product = _productRepository.FindByCode(dto.ProductCode);
            if(product == null)
            {
                throw new ProductNotFoundException();
            }

            var purchaseBill = new PurchaseBill
            {
                Count = dto.Count,
                WholePrice = dto.WholePrice,
                Date = dto.Date,
                SellerName = dto.SellerName,
                Product = product,
                ProductId = product.Id
            };

            _repository.Add(purchaseBill);
            product.InStockCount += dto.Count;
            _unitOfWork.Commit();
        }

        public void Delete(int billId)
        {
            var purchaseBill = _repository.FindById(billId);
            if(purchaseBill == null)
            {
                throw new PurchaseBillNotFoundException();
            }
            _productRepository
                .RemoveFromStock(purchaseBill.ProductId, purchaseBill.Count);
            _repository.Delete(purchaseBill);
            _unitOfWork.Commit();
        }

        public GetPurchaseBillDto Get(int id)
        {
            var isBillExist = _repository.IsExistPurchaseBill(id);
            if (!isBillExist)
            {
                throw new PurchaseBillNotFoundException();
            }
            return _repository.Get(id);
        }

        public IList<GetPurchaseBillDto> GetAll()
        {
            return _repository.GetAll();
        }

        public void Update(int id, UpdatePurchaseBillDto dto)
        {
            var product = _productRepository.FindByCode(dto.ProductCode);

            PurchaseBill purchaseBill = _repository.FindById(id);
            if(purchaseBill == null)
            {
                throw new PurchaseBillNotFoundException();
            }

            Product previousProduct = _productRepository
                .FindById(purchaseBill.ProductId);

            _productRepository.RemoveFromStock(previousProduct.Id, dto.Count);
            purchaseBill.Product = product;
            purchaseBill.ProductId = product.Id;
            purchaseBill.Count = dto.Count;
            purchaseBill.SellerName = dto.SellerName;
            purchaseBill.WholePrice = dto.WholePrice;
            _productRepository.AddtoStock(product.Id, dto.Count);
            _unitOfWork.Commit();
        }
    }
}
