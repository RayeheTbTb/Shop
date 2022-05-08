using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.PurchaseBills.Exceptions;
using Shop.Test.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Shop.Services.Test.Unit.PurchaseBills
{
    public class PurchaseBillServiceTests
    {
        private readonly EFDataContext _dataContext;
        private readonly PurchaseBillService _sut;
        private readonly UnitOfWork _unitOfWork;
        private readonly PurchaseBillRepository _repository;
        private readonly ProductRepository _productRepository;

        public PurchaseBillServiceTests()
        {
            _dataContext = new EFInMemoryDatabase().CreateDataContext<EFDataContext>();
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _repository = new EFPurchaseBillRepository(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new PurchaseBillAppService(_repository, _unitOfWork, _productRepository);
        }

        [Fact]
        public void Update_updates_purchaseBill_and_related_products_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithPurchaseBill("Seller", 10,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 10000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2).Build();
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            var billId = product.PurchaseBills.First().Id;
            UpdatePurchaseBillDto dto = GenerateUpdatePurchaseBillDto(product2);

            _sut.Update(billId, dto);

            var expected = _dataContext.PurchaseBills.FirstOrDefault(_ => _.Id == billId);
            expected.ProductId.Should().Be(product2.Id);
            expected.SellerName.Should().Be(dto.SellerName);
            expected.Product.Name.Should().Be(product2.Name);
            expected.Count.Should().Be(dto.Count);
            expected.WholePrice.Should().Be(dto.WholePrice);
            product.PurchaseBills.Should().NotContain(_ => _.Id == billId);
            product.InStockCount.Should().Be(5);
        }

        [Theory]
        [InlineData(2)]
        public void Update_throws_PurchaseBillNotFoundException_when_purchaseBill_with_given_id_does_not_exist(int fakeBillId)
        {

            UpdatePurchaseBillDto dto = GenerateUpdatePurchaseBillDto();

            Action expected = () => _sut.Update(fakeBillId, dto);

            expected.Should().ThrowExactly<PurchaseBillNotFoundException>();

        }

        private static UpdatePurchaseBillDto GenerateUpdatePurchaseBillDto()
        {
            return new UpdatePurchaseBillDto
            {
                ProductName = "dummy",
                Count = 5,
                WholePrice = 5000,
                SellerName = "Seller2",
                ProductCode = 2
            };
        }

        private static UpdatePurchaseBillDto GenerateUpdatePurchaseBillDto(Product newProduct)
        {
            return new UpdatePurchaseBillDto
            {
                ProductName = newProduct.Name,
                Count = 5,
                WholePrice = 5000,
                SellerName = "Seller2",
                ProductCode = newProduct.Code
            };
        }
    }
}

