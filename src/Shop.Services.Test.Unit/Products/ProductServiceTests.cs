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
            var product = ProductFactory.CreateProduct(category);
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
            var product = ProductFactory.CreateProduct(category, "dummy", 1);
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
            var product = ProductFactory.CreateProduct(category);
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

            expected.Should().ThrowExactly<ProductDoesNotExistException>();
        }

        [Fact]
        public void Delete_deletes_product_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = ProductFactory.CreateProduct(category);
            ProductFactory.AddProductToDatabase(product, _dataContext);

            _sut.Delete(product.Code);

            _dataContext.Products.Should().NotContain(_ => _.Code == product.Code);
        }

        [Theory]
        [InlineData(2)]
        public void Delete_throws_ProductDoesNotExistException_when_product_with_given_code_does_not_exist(int fakeCode)
        {
            Action expected = () => _sut.Delete(fakeCode);

            expected.Should().ThrowExactly<ProductDoesNotExistException>();
        }

        [Fact]
        public void Delete_throws_UnableToDeleteProductWithExistingPurchaseBillException_when_there_is_a_purchaseBill_for_the_product_with_given_code()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = ProductFactory.CreateProduct(category);
            ProductFactory.AddProductToDatabase(product, _dataContext);
            PurchaseBill purchaseBill = CreatePurchaseBill(product);
            _dataContext.Manipulate(_ => _.PurchaseBills.Add(purchaseBill));

            Action expected = () => _sut.Delete(product.Code);

            expected.Should().ThrowExactly<UnableToDeleteProductWithExistingPurchaseBillException>();
        }

        [Fact]
        public void Update_updates_product_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = ProductFactory.CreateProduct(category);
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

            expected.Should().ThrowExactly<ProductDoesNotExistException>();
        }

        [Fact]
        public void Update_throws_DuplicateProductNameInCategoryException_when_given_name_already_exists_in_the_category()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = ProductFactory.CreateProduct(category, "UpdatedDummy", 1);
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = ProductFactory.CreateProduct(category, "dummy2", 2);
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            UpdateProductDto dto = GenerateUpdateProductDto();

            Action expected = () => _sut.Update(product2.Code, dto);

            expected.Should().ThrowExactly<DuplicateProductNameInCategoryException>();
        }

        private static UpdateProductDto GenerateUpdateProductDto()
        {
            return new UpdateProductDto
            {
                Name = "UpdatedDummy",
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
