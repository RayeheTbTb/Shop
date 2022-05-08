using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
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
        public void Add_adds_purchaseBill_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk")
                .WithCode(1).WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            AddPurchaseBillDto dto = GenerateAddPurchaseBillDto(product.Code);

            _sut.Add(dto);

            var expected = _dataContext.PurchaseBills.FirstOrDefault();
            expected.SellerName.Should().Be(dto.SellerName);
            expected.Count.Should().Be(dto.Count);
            expected.ProductId.Should().Be(product.Id);
            expected.WholePrice.Should().Be(dto.WholePrice);
            expected.Date.Date.Should().Be(dto.Date.Date);
            var expectedProduct = _dataContext.Products.Where(_ => _.Id == product.Id).FirstOrDefault();
            expectedProduct.InStockCount.Should().Be(20);
        }

        [Theory]
        [InlineData(2)]
        public void Add_throws_ProductNotFoundException_when_product_with_given_code_does_not_exist(int fakeProductCode)
        {
            var dto = GenerateAddPurchaseBillDto(fakeProductCode);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void Update_updates_purchaseBill_and_related_products_properly()
        {
            var category = MakeCategoryInDatabase();
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

        [Fact]
        public void Delete_deletes_purchaseBill_properly()
        {
            var category = MakeCategoryInDatabase();
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithPurchaseBill("Seller", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var billId = product.PurchaseBills.First().Id;

            _sut.Delete(billId);

            _dataContext.PurchaseBills.Should().NotContain(_ => _.Id == billId);
            _dataContext.Products.FirstOrDefault(_ => _.Id == product.Id)
                .InStockCount.Should().Be(5);
        }

        [Theory]
        [InlineData(2)]
        public void Delete_throws_PurchaseBillNotFoundException_when_purchaseBill_with_given_id_does_not_exist(int fakeId)
        {
            Action expected = () => _sut.Delete(fakeId);
            expected.Should().ThrowExactly<PurchaseBillNotFoundException>();
        }

        [Fact]
        public void GetAll_returns_all_purchaseBills()
        {
            var category = MakeCategoryInDatabase();
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithPurchaseBill("Seller", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            var expected = _sut.GetAll();

            var purchasebill = product.PurchaseBills.First();
            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.ProductId == product.Id);
            expected.Should().Contain(_ => _.SellerName == purchasebill.SellerName);
            expected.Should().Contain(_ => _.Date.Date == purchasebill.Date.Date);
            expected.Should().Contain(_ => _.Count == purchasebill.Count);
            expected.Should().Contain(_ => _.WholePrice == purchasebill.WholePrice);
        }

        [Fact]
        public void Get_returns_purchaseBill_with_given_id()
        {
            var category = MakeCategoryInDatabase();
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithPurchaseBill("Seller", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var billId = product.PurchaseBills.First().Id;

            var expected = _sut.Get(billId);

            var purchasebill = product.PurchaseBills.First();
            expected.ProductId.Should().Be(product.Id);
            expected.SellerName.Should().Be(purchasebill.SellerName);
            expected.Date.Date.Should().Be(purchasebill.Date.Date);
            expected.Count.Should().Be(purchasebill.Count);
            expected.WholePrice.Should().Be(purchasebill.WholePrice);
        }

        [Theory]
        [InlineData(2)]
        public void Get_throws_PurchaseBillNotFoundException_when_purchaseBill_with_given_id_does_not_exist(int fakeId)
        {
            Action expected = () => _sut.Get(fakeId);

            expected.Should().ThrowExactly<PurchaseBillNotFoundException>();
        }

        private static AddPurchaseBillDto GenerateAddPurchaseBillDto(int productCode)
        {
            return new AddPurchaseBillDto
            {
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                ProductCode = productCode,
                SellerName = "seller",
                WholePrice = 10000
            };
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

        private Category MakeCategoryInDatabase()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            return category;
        }
    }
}

