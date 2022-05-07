using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Specs.Infrastructure;
using Shop.Test.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.Products
{
    [Scenario("مشاهده کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class GetProductWithPurchaseBills : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        private GetProductWithPurchaseBillsDto expected;
        public GetProductWithPurchaseBills(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10' و یک سند خرید وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = new ProductBuilder(_category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000)
                .WithPurchaseBill().Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("درخواست مشاهده اسناد خرید کالا با کد محصول '1' را میدهم")]
        public void When()
        {
            expected = _sut.GetPurchaseBills(_product.Code);
        }

        [Then("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10'و یک سند خرید باید نمایش داده شود")]
        public void Then()
        {
            var purchaseBills = _product.PurchaseBills.FirstOrDefault();
            expected.Id.Should().Be(_product.Id);
            expected.Name.Should().Be( _product.Name);
            expected.InStockCount.Should().Be(_product.InStockCount);
            expected.Code.Should().Be(_product.Code);
            expected.Price.Should().Be(_product.Price);
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

        [Fact]
        public void Run()
        {
            Given();
            When();
            Then();
        }
    }
}
