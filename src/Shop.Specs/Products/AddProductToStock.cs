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
    [Scenario("ورود کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class AddProductToStock : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        private AddProductToStockDto _dto;
        PurchaseBill expected;
        public AddProductToStock(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '10' عدد در فهرست کالا ها وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = ProductFactory.CreateProduct(_category);
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [And("هیچ سند ورود کالایی در فهرست سند ورودی کالا وجود ندارد")]
        public void GivenAnd()
        {

        }

        [When("کالایی با کد '1' با تعداد '10' در تاریخ '21 / 02 / 1400' وارد میکنیم")]
        public void When()
        {
            _dto = new AddProductToStockDto
            {
                Code = _product.Code,
                CategoryId = _category.Id,
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                SellerName = "SellerName",
                WholePrice = 10000
            };
            _sut.AddToStock(_dto);
        }

        [Then("سند ورود کالایی با کد '1' با تعداد '10' در تاریخ '21 / 02 / 1400' در فهرست سند ورودی کالا باید وجود داشته باشد")]
        public void Then()
        {
            expected = _dataContext.PurchaseBills.FirstOrDefault();
            expected.Product.Code.Should().Be(_dto.Code);
            expected.Product.CategoryId.Should().Be(_dto.CategoryId);
            expected.Count.Should().Be(_dto.Count);
            expected.Date.Should().Be(_dto.Date);
            expected.SellerName.Should().Be(_dto.SellerName);
            expected.WholePrice.Should().Be(_dto.WholePrice);
        }

        [And("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '20' عدد در فهرست کالا ها باید وجود داشته باشد")]
        public void ThenAnd()
        {
            expected.Product.InStockCount.Should().Be(20);
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
