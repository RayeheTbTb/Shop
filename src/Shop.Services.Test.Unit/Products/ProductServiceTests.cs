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
            var dto= 
                GenerateDefineProductDtoWithSpecificCode(category,product.Code);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<DuplicateProductCodeException>();
        }

        [Fact]
        public void Add_throws_DuplicateProductNameInCategoryException_when_product_name_already_exists_in_category()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = ProductFactory.CreateProduct(category, "dummy");
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
                GenerateAddProductToStockDto(category, product);

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

        private static AddProductToStockDto GenerateAddProductToStockDto(Category category, Product product)
        {
            return new AddProductToStockDto
            {
                CategoryId = category.Id,
                Code = product.Code,
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
