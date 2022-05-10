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
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.PurchaseBills
{
    [Scenario("نمایش اسناد خرید")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " سند خرید را مدیریت کنم",
        InOrderTo = "سند خرید خود را تعریف کنم"
    )]
    public class GetAllPurchaseBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly PurchaseBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly PurchaseBillService _sut;
        private Product _product;
        private IList<GetPurchaseBillDto> expected;
        public GetAllPurchaseBill(ConfigurationFixture configuration) : base(configuration)
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
        }

        [When("درخواست مشاهده فهرست اسناد ورودی کالا ها را میدهم")]
        public void When()
        {
            expected = _sut.GetAll();
        }

        [Then("سند خریدی به تاریخ '01 / 01 / 1400' به نام 'فروشنده' برای کالای با عنوان 'شیر کاله' به تعداد '5' با مجموع قیمت '5000 تومان' باید نمایش داده شود")]
        public void Then()
        {
            var purchasebill = _product.PurchaseBills.First();
            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.ProductId == _product.Id);
            expected.Should().Contain(_ => _.Count == purchasebill.Count);
            expected.Should()
                .Contain(_ => _.SellerName == purchasebill.SellerName);
            expected.Should()
                .Contain(_ => _.Date.Date == purchasebill.Date.Date);
            expected.Should()
                .Contain(_ => _.WholePrice == purchasebill.WholePrice);
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
