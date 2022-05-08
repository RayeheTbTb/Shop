using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products;
using Shop.Services.Products.Contracts;
using Shop.Services.Products.Exceptions;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.PurchaseBills.Exceptions;
using Shop.Services.SaleBills.Exceptions;
using Shop.Test.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Shop.Services.Test.Unit.Products
{
    public class ProductServiceTests
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductService _sut;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;

        public ProductServiceTests()
        {
            _dataContext = new EFInMemoryDatabase().CreateDataContext<EFDataContext>();
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _repository = new EFProductRepository(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Fact]
        public void Add_adds_product_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var dto = GenerateDefineProductDto(category);

            _sut.Add(dto);

            var expected = _dataContext.Products.FirstOrDefault();
            expected.Name.Should().Be(dto.Name);
            expected.Code.Should().Be(dto.Code);
            expected.MinimumInStock.Should().Be(dto.MinimumInStock);
            expected.CategoryId.Should().Be(dto.CategoryId);
        }

        [Fact]
        public void Add_throws_DuplicateProductCodeException_when_product_with_given_code_already_exists()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var dto =
                GenerateDefineProductDtoWithSpecificCode(category, product.Code);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<DuplicateProductCodeException>();
        }

        [Fact]
        public void Add_throws_DuplicateProductNameInCategoryException_when_product_name_already_exists_in_category()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("dummy").WithCode(1).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var dto = GenerateDefineProductDto(category);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<DuplicateProductNameInCategoryException>();
        }

        [Fact]
        public void AddToStock_adds_the_given_count_to_product_InStockCount_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            AddProductToStockDto dto =
                GenerateAddProductToStockDto(category, product.Code);

            _sut.AddToStock(dto);

            _dataContext.Products
                .FirstOrDefault(_ => _.Code == product.Code).InStockCount
                .Should().Be(20);
            var expected = _dataContext.PurchaseBills.FirstOrDefault();
            expected.SellerName.Should().Be(dto.SellerName);
            expected.Date.Should().Be(dto.Date);
            expected.WholePrice.Should().Be(dto.WholePrice);
            expected.Count.Should().Be(dto.Count);
            expected.Product.Code.Should().Be(dto.Code);
        }

        [Theory]
        [InlineData(2)]
        public void AddToStock_throws_ProductDoesNotExistException_when_product_with_given_code_does_not_exist(int fakeCode)
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            AddProductToStockDto dto =
                GenerateAddProductToStockDto(category, fakeCode);

            Action expected = () => _sut.AddToStock(dto);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void Delete_deletes_product_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            _sut.Delete(product.Code);

            _dataContext.Products.Should()
                .NotContain(_ => _.Code == product.Code);
        }

        [Theory]
        [InlineData(2)]
        public void Delete_throws_ProductDoesNotExistException_when_product_with_given_code_does_not_exist(int fakeCode)
        {
            Action expected = () => _sut.Delete(fakeCode);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void Delete_throws_UnableToDeleteProductWithExistingPurchaseBillException_when_there_is_a_purchaseBill_for_the_product_with_given_code()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithPurchaseBill().Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            Action expected = () => _sut.Delete(product.Code);

            expected.Should().ThrowExactly<UnableToDeleteProductWithExistingPurchaseBillException>();
        }

        [Fact]
        public void Update_updates_product_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            UpdateProductDto dto = GenerateUpdateProductDto();

            _sut.Update(product.Code, dto);

            var expected = _dataContext.Products.FirstOrDefault(_ => _.Code == product.Code);
            expected.Name.Should().Be(dto.Name);
            expected.MinimumInStock.Should().Be(dto.MinimumInStock);
        }

        [Theory]
        [InlineData(2)]
        public void Update_throws_ProductDoesNotExistException_when_product_with_given_code_does_not_exist(int fakeCode)
        {
            UpdateProductDto dto = GenerateUpdateProductDto();

            Action expected = () => _sut.Update(fakeCode, dto);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void Update_throws_DuplicateProductNameInCategoryException_when_given_name_already_exists_in_the_category()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("dummy1").WithCode(1).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = new ProductBuilder(category)
                .WithName("dummy2").WithCode(2).Build();
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            UpdateProductDto dto = GenerateUpdateProductDto("dummy1");

            Action expected = () => _sut.Update(product2.Code, dto);

            expected.Should().ThrowExactly<DuplicateProductNameInCategoryException>();
        }

        [Fact]
        public void GetAll_returns_all_products()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            var expected = _sut.GetAll();

            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.Id == product.Id);
            expected.Should().Contain(_ => _.Name == product.Name);
            expected.Should()
                .Contain(_ => _.InStockCount == product.InStockCount);
            expected.Should().Contain(_ => _.Code == product.Code);
            expected.Should().Contain(_ => _.Price == product.Price);
        }

        [Fact]
        public void GetPurchaseBills_returns_product_with_given_code_and_its_purchase_bills()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000)
                .WithPurchaseBill().Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            var expected = _sut.GetPurchaseBills(product.Code);

            var purchaseBills = product.PurchaseBills.FirstOrDefault();
            expected.Id.Should().Be(product.Id);
            expected.Name.Should().Be(product.Name);
            expected.InStockCount.Should().Be(product.InStockCount);
            expected.Code.Should().Be(product.Code);
            expected.Price.Should().Be(product.Price);
            expected.PurchaseBills.Should().HaveCount(1);
            expected.PurchaseBills.Should()
                .Contain(_ => _.SellerName == purchaseBills.SellerName);
            expected.PurchaseBills.Should()
                .Contain(_ => _.Id == purchaseBills.Id);
            expected.PurchaseBills.Should()
                .Contain(_ => _.Count == purchaseBills.Count);
            expected.PurchaseBills.Should()
                .Contain(_ => _.Date.Date == purchaseBills.Date.Date);
            expected.PurchaseBills.Should()
                .Contain(_ => _.WholePrice == purchaseBills.WholePrice);
        }

        [Theory]
        [InlineData(2)]
        public void GetPurchaseBills_throws_ProductDoesNotExistException_when_product_with_given_code_does_not_exist(int fakeCode)
        {
            Action expected = () => _sut.GetPurchaseBills(fakeCode);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void GetPurchaseBills_throws_PurchaseBillNotFoundException_when_no_purchase_bills_exist_for_the_product_with_given_code()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            Action expected = () => _sut.GetPurchaseBills(product.Code);

            expected.Should().ThrowExactly<PurchaseBillNotFoundException>();
        }

        [Fact]
        public void GetSaleBills_returns_product_with_given_code_and_its_sale_bills()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000)
                .WithSaleBill().Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            var expected = _sut.GetSaleBills(product.Code);

            var saleBills = product.SaleBills.FirstOrDefault();
            expected.Id.Should().Be(product.Id);
            expected.Name.Should().Be(product.Name);
            expected.InStockCount.Should().Be(product.InStockCount);
            expected.Code.Should().Be(product.Code);
            expected.Price.Should().Be(product.Price);
            expected.SaleBills.Should().HaveCount(1);
            expected.SaleBills.Should()
                .Contain(_ => _.CustomerName == saleBills.CustomerName);
            expected.SaleBills.Should().Contain(_ => _.Id == saleBills.Id);
            expected.SaleBills.Should()
                .Contain(_ => _.Count == saleBills.Count);
            expected.SaleBills.Should()
                .Contain(_ => _.Date.Date == saleBills.Date.Date);
            expected.SaleBills.Should()
                .Contain(_ => _.WholePrice == saleBills.WholePrice);
        }

        [Theory]
        [InlineData(2)]
        public void GetSaleBills_throws_ProductDoesNotExistException_when_product_with_given_code_does_not_exist(int fakeCode)
        {
            Action expected = () => _sut.GetSaleBills(fakeCode);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void GetSaleBills_throws_NoSaleBillsExistException_when_no_sale_bills_exist_for_the_product_with_given_code()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);

            Action expected = () => _sut.GetSaleBills(product.Code);

            expected.Should().ThrowExactly<NoSaleBillsExistException>();
        }

        private static UpdateProductDto GenerateUpdateProductDto()
        {
            return new UpdateProductDto
            {
                Name = "UpdatedDummy",
                MinimumInStock = 10,
            };
        }

        private static UpdateProductDto GenerateUpdateProductDto(string name)
        {
            return new UpdateProductDto
            {
                Name = name,
                MinimumInStock = 10,
            };
        }

        private static PurchaseBill CreatePurchaseBill(Product product)
        {
            return new PurchaseBill
            {
                Product = product,
                ProductId = product.Id,
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                SellerName = "name",
                WholePrice = 10000,
            };
        }

        private static AddProductToStockDto GenerateAddProductToStockDto(Category category, int productCode)
        {
            return new AddProductToStockDto
            {
                CategoryId = category.Id,
                Code = productCode,
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                SellerName = "dummy",
                WholePrice = 10000
            };
        }

        private static DefineProductDto GenerateDefineProductDto(Category category)
        {
            return new DefineProductDto
            {
                Name = "dummy",
                Code = 2,
                MinimumInStock = 1,
                CategoryId = category.Id
            };
        }

        private static DefineProductDto GenerateDefineProductDtoWithSpecificCode(Category category, int code)
        {
            return new DefineProductDto
            {
                Name = "dummy",
                Code = code,
                MinimumInStock = 1,
                CategoryId = category.Id
            };
        }
    }
}
