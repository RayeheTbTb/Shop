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
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.PurchaseBills
{
    [Scenario("اضافه کردن سند خرید")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " سند خرید را مدیریت کنم",
        InOrderTo = "سند خرید خود را تعریف کنم"
    )]
    public class AddPurchaseBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly PurchaseBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly PurchaseBillService _sut;
        private Product _product;
        private AddPurchaseBillDto _dto;
        public AddPurchaseBill(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFPurchaseBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new PurchaseBillAppService(_repository, _unitOfWork, _productRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '10' عدد در فهرست کالا ها وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk")
                .WithCode(1).WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [And("هیچ سند ورود کالایی در فهرست سند ورودی کالا وجود ندارد")]
        public void GivenAnd()
        {

        }

        [When("سند کالایی برای کالایی به نام 'فروشنده' با کد '1' با تعداد '10' با مجموع قیمت '5000 تومان' در تاریخ '21 / 02 / 1400' وارد میکنیم")]
        public void When()
        {
            _dto = new AddPurchaseBillDto
            {
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                ProductCode = _product.Code,
                SellerName = "seller",
                WholePrice = 10000
            };

            _sut.Add(_dto);
        }

        [Then("سند ورود کالایی با کد '1' با تعداد '10' در تاریخ '21 / 02 / 1400' در فهرست سند ورودی کالا باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.PurchaseBills.FirstOrDefault();
            expected.SellerName.Should().Be(_dto.SellerName);
            expected.Count.Should().Be(_dto.Count);
            expected.ProductId.Should().Be(_product.Id);
            expected.WholePrice.Should().Be(_dto.WholePrice);
            expected.Date.Date.Should().Be(_dto.Date.Date);

        }

        [And("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '20' عدد در فهرست کالا ها باید وجود داشته باشد")]
        public void ThenAnd()
        {
            var expectedProduct = _dataContext.Products.Where(_ => _.Id == _product.Id).FirstOrDefault();
            expectedProduct.InStockCount.Should().Be(20);
        }

        [Fact]
        public void Run()
        {
            Given();
            GivenAnd();
            When();
            Then();
            ThenAnd();
        }
    }
}
