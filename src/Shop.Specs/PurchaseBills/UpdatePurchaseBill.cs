using Shop.Specs.Infrastructure;
using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using static Shop.Specs.BDDHelper;
using Shop.Persistence.EF;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Infrastructure.Application;
using Shop.Services.Products.Contracts;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Persistence.EF.Products;
using Shop.Services.PurchaseBills;
using Shop.Test.Tools;
using Shop.Entities;

namespace Shop.Specs.PurchaseBills
{
    [Scenario("ویرایش سند خرید")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " سند خرید را مدیریت کنم",
        InOrderTo = "سند خرید خود را تعریف کنم"
    )]
    public class UpdatePurchaseBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly PurchaseBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly PurchaseBillService _sut;
        private Product _product;
        private Product _product2;
        private UpdatePurchaseBillDto _dto;
        private int _billId;
        public UpdatePurchaseBill(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFPurchaseBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new PurchaseBillAppService(_repository, _unitOfWork, 
                _productRepository);
        }

        [Given("سند خریدی به تاریخ '01 / 01 / 1400' به نام 'فروشنده' برای کالای با عنوان 'شیر کاله' به تعداد '10' با مجموع قیمت '10000 تومان' فهرست سند ورودی کالا وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithPurchaseBill("Seller", 10,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 10000).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
            _product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2).Build();
            ProductFactory.AddProductToDatabase(_product2, _dataContext);
            _billId = _product.PurchaseBills.First().Id;
        }

        [When("نام را به 'فروشنده2'، عنوان کالا را به 'ماست کاله' تعداد را به '5' و مجموع قیمت را به '5000 تومان' تغییر میدهم")]
        public void When()
        {
            _dto = new UpdatePurchaseBillDto
            {
                ProductName = _product2.Name,
                Count = 5,
                WholePrice = 5000,
                SellerName = "Seller2",
                ProductCode = _product2.Code
            };

            _sut.Update(_billId, _dto);
        }

        [Then("سند خریدی به نام 'فروشنده2' برای کالای با عنوان 'ماست کاله' به تعداد '5' با مجموع قیمت '5000 تومان' در فهرست سند ورودی کالا باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.PurchaseBills
                .FirstOrDefault(_ => _.Id == _billId);

            expected.ProductId.Should().Be(_product2.Id);
            expected.SellerName.Should().Be(_dto.SellerName);
            expected.Product.Name.Should().Be(_product2.Name);
            expected.Count.Should().Be(_dto.Count);
            expected.WholePrice.Should().Be(_dto.WholePrice);
            _product.PurchaseBills.Should().NotContain(_ => _.Id == _billId);
            _product.InStockCount.Should().Be(5);
            _product2.InStockCount.Should().Be(15);
        }

        [Fact]
        public void Run()
        {
            Given();
            When();
            Then();
        }
    }
}
