using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.PurchaseBills.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.PurchaseBills
{
    public class PurchaseBillAppService : PurchaseBillService
    {
        private readonly PurchaseBillRepository _repository;
        private readonly ProductRepository _productRepository;
        private readonly UnitOfWork _unitOfWork;

        public PurchaseBillAppService(PurchaseBillRepository repository, UnitOfWork unitOfWork, ProductRepository productRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public void Delete(int billId)
        {
            var purchaseBill = _repository.FindById(billId);
            if(purchaseBill == null)
            {
                throw new PurchaseBillNotFoundException();
            }
            _productRepository
                .RemoveFromStock(purchaseBill.Product, purchaseBill.Count);
            _repository.Delete(purchaseBill);
            _unitOfWork.Commit();
        }

        public void Update(int id, UpdatePurchaseBillDto dto)
        {
            var product = _productRepository.FindByCode(dto.ProductCode);
            PurchaseBill purchaseBill = _repository.FindById(id);
            if(purchaseBill == null)
            {
                throw new PurchaseBillNotFoundException();
            }
            Product previousProduct = _productRepository.FindById(purchaseBill.ProductId);
            //_productRepository.RemovePurchaseBill(purchaseBill.ProductId, id);
            //previousProduct.PurchaseBills.Remove(_ => _.Id == purchaseBill.Id);
            _productRepository.RemoveFromStock(previousProduct, dto.Count);
            purchaseBill.Product = product;
            purchaseBill.ProductId = product.Id;
            purchaseBill.Count = dto.Count;
            purchaseBill.SellerName = dto.SellerName;
            purchaseBill.WholePrice = dto.WholePrice;
            _productRepository.AddtoStock(dto.ProductCode, dto.Count);
            _unitOfWork.Commit();
        }
    }
}
