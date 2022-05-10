using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.SaleBills;
using Shop.Services.Products.Contracts;
using Shop.Services.SaleBills;
using Shop.Services.SaleBills.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Shop.Test.Tools;
using Shop.Entities;
using Shop.Services.Products.Exceptions;
using Shop.Services.SaleBills.Exceptions;

namespace Shop.Services.Test.Unit.SaleBills
{
    public class SaleBillServiceTests
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillService _sut;
        private readonly UnitOfWork _unitOfWork;
        private readonly SaleBillRepository _repository;
        private readonly ProductRepository _productRepository;

        public SaleBillServiceTests()
        {
            _dataContext = new EFInMemoryDatabase()
                .CreateDataContext<EFDataContext>();
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _repository = new EFSaleBillRepository(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new SaleBillAppService(_repository, _unitOfWork,
                _productRepository);
        }

        [Fact]
        public void Add_adds_saleBill_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk")
                .WithCode(1).WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            AddSaleBillDto dto = GenerateAddSaleBillDto(product.Code);

            _sut.Add(dto);

            var expected = _dataContext.SaleBills.FirstOrDefault();
            expected.CustomerName.Should().Be(dto.CustomerName);
            expected.Count.Should().Be(dto.Count);
            expected.ProductId.Should().Be(product.Id);
            expected.WholePrice.Should().Be(dto.WholePrice);
            expected.Date.Date.Should().Be(dto.Date.Date);
            var expectedProduct = _dataContext.Products
                .Where(_ => _.Id == product.Id).FirstOrDefault();
            expectedProduct.InStockCount.Should().Be(5);
        }

        [Theory]
        [InlineData(2)]
        public void Add_throws_ProductNotFoundException_when_product_with_given_code_does_not_exist(int fakeProductCode)
        {
            var dto = GenerateAddSaleBillDto(fakeProductCode);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<ProductNotFoundException>();
        }

        [Fact]
        public void Add_throws_NotEnoughProductInStockException_when_saleBill_count_is_bigger_than_product_inStockCount()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk")
                .WithCode(1).WithInStockCount(4).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            AddSaleBillDto dto = GenerateAddSaleBillDto(product.Code);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<NotEnoughProductInStockException>();
        }

        [Fact]
        public void Add_throws_ProductOutOfStockException_when_product_becomes_outOfStock_after_the_sale()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk")
                .WithCode(1).WithInStockCount(5).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            AddSaleBillDto dto = GenerateAddSaleBillDto(product.Code);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<ProductOutOfStockException>();
        }

        [Fact]
        public void Add_throws_ProductReachedMinimumInStockCountException_when_product_inStockCount_becomes_less_than_minimumInStockCount_after_sale()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk")
                .WithCode(1).WithInStockCount(10)
                .WithMinimumInStock(6).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            AddSaleBillDto dto = GenerateAddSaleBillDto(product.Code);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<ProductReachedMinimumInStockCountException>();
        }

        [Fact]
        public void Delete_deletes_saleBill_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("dummy").WithCode(1)
                .WithInStockCount(5)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var billId = product.SaleBills.First().Id;

            _sut.Delete(billId);

            _dataContext.SaleBills.Should().NotContain(_ => _.Id == billId);
            _dataContext.Products.FirstOrDefault(_ => _.Id == product.Id)
               .InStockCount.Should().Be(10);
        }

        [Theory]
        [InlineData(2)]
        public void Delete_throws_SaleBillNotFoundException_when_no_saleBill_with_given_id_exists(int fakeId)
        {
            Action expected = () => _sut.Delete(fakeId);

            expected.Should().ThrowExactly<SaleBillNotFoundException>();
        }

        [Fact]
        public void Update_updates_saleBill_properly()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithInStockCount(5)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2)
                .WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            var billId = product.SaleBills.First().Id;
            UpdateSaleBillDto dto = GenerateUpdateSaleBillDto(product2);

            _sut.Update(billId, dto);

            var expected = _dataContext.SaleBills.FirstOrDefault(_ => _.Id == billId);

            expected.ProductId.Should().Be(product2.Id);
            expected.CustomerName.Should().Be(dto.CustomerName);
            expected.Product.Name.Should().Be(product2.Name);
            expected.Count.Should().Be(dto.Count);
            expected.WholePrice.Should().Be(dto.WholePrice);
            product.SaleBills.Should().NotContain(_ => _.Id == billId);
            product.InStockCount.Should().Be(10);
            product2.InStockCount.Should().Be(5);
        }

        [Fact]
        public void Update_throws_NotEnoughProductInStockException_when_saleBill_count_is_bigger_than_product_inStockCount()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithInStockCount(5)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2)
                .WithInStockCount(4).Build();
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            var billId = product.SaleBills.First().Id;
            UpdateSaleBillDto dto = GenerateUpdateSaleBillDto(product2);

            Action expected = () => _sut.Update(billId, dto);

            expected.Should().ThrowExactly<NotEnoughProductInStockException>();
        }

        [Fact]
        public void Update_throws_ProductOutOfStockException_when_product_becomes_outOfStock_after_the_sale()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithInStockCount(5)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2)
                .WithInStockCount(5).Build();
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            var billId = product.SaleBills.First().Id;
            UpdateSaleBillDto dto = GenerateUpdateSaleBillDto(product2);

            Action expected = () => _sut.Update(billId, dto);

            expected.Should().ThrowExactly<ProductOutOfStockException>();
        }

        [Fact]
        public void Update_throws_ProductReachedMinimumInStockCountException_when_product_inStockCount_becomes_less_than_minimumInStockCount_after_sale()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            var product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithInStockCount(5)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(product, _dataContext);
            var product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2)
                .WithMinimumInStock(6)
                .WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(product2, _dataContext);
            var billId = product.SaleBills.First().Id;
            UpdateSaleBillDto dto = GenerateUpdateSaleBillDto(product2);

            Action expected = () => _sut.Update(billId, dto);

            expected.Should().ThrowExactly<ProductReachedMinimumInStockCountException>();
        }

        private static UpdateSaleBillDto GenerateUpdateSaleBillDto(Product product)
        {
            return new UpdateSaleBillDto
            {
                ProductName = product.Name,
                Count = 5,
                WholePrice = 5000,
                CustomerName = "Seller2",
                ProductCode = product.Code
            };
        }

        private static AddSaleBillDto GenerateAddSaleBillDto(int productCode)
        {
            return new AddSaleBillDto
            {
                Count = 5,
                CustomerName = "dummy",
                ProductCode = productCode,
                WholePrice = 5000,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z")
            };
        }
    }
}
