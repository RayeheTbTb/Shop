using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Specs.Infrastructure;
using Shop.Test.Tools;
using System;
using System.Linq;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.PurchaseBills
{
    [Scenario("نمایش سند خرید")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " سند خرید را مدیریت کنم",
        InOrderTo = "سند خرید خود را تعریف کنم"
    )]
    public class GetPurchaseBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly PurchaseBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly PurchaseBillService _sut;
        private Product _product;
        private GetPurchaseBillDto expected;
        private int _billId;

        public GetPurchaseBill(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFPurchaseBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new PurchaseBillAppService(_repository, _unitOfWork, 
                _productRepository);
        }

        [Given("سند خریدی به تاریخ '01 / 01 / 1400' به نام 'فروشنده' برای کالای با عنوان 'شیر کاله' به تعداد '5' با مجموع قیمت '5000 تومان' در فهرست سند ورودی کالا وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithPurchaseBill("Seller", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
            _billId = _product.PurchaseBills.First().Id;
        }

        [When("درخواست مشاهده سند خرید کالا را میدهم")]
        public void When()
        {
            expected = _sut.Get(_billId);
        }

        [Then("سند خریدی به تاریخ '01 / 01 / 1400' به نام 'فروشنده' برای کالای با عنوان 'شیر کاله' به تعداد '5' با مجموع قیمت '5000 تومان' باید نمایش داده شود")]
        public void Then()
        {
            var purchasebill = _product.PurchaseBills.First();
            expected.ProductId.Should().Be(_product.Id);
            expected.SellerName.Should().Be(purchasebill.SellerName);
            expected.Date.Date.Should().Be(purchasebill.Date.Date);
            expected.Count.Should().Be(purchasebill.Count);
            expected.WholePrice.Should().Be(purchasebill.WholePrice);
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
