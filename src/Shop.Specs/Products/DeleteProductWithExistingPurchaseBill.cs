using Shop.Specs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using static Shop.Specs.BDDHelper;
using Shop.Persistence.EF;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products;
using Shop.Test.Tools;
using Shop.Entities;
using Shop.Infrastructure.Test;
using Shop.Services.Products.Exceptions;

namespace Shop.Specs.Products
{
    [Scenario("حذف کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class DeleteProductWithExistingPurchaseBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        Action expected;
        public DeleteProductWithExistingPurchaseBill(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله' و کد کالای '1' تعریف شده است")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = ProductFactory.CreateProduct(_category);
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [And("یک سند ورود برای کالا با عنوان 'شیر کاله' و کد کالای '1' وجود دارد")]
        public void GivenAnd()
        {
            var purchaseBill = new PurchaseBill
            {
                Product = _product,
                ProductId = _product.Id,
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                SellerName = "name",
                WholePrice = 10000,
            };
            _dataContext.Manipulate(_ => _.PurchaseBills.Add(purchaseBill));
        }

        [When("کالایی با عنوان 'شیر کاله' و کد کالای '1' را حذف میکنم")]
        public void When()
        {
            expected = () => _sut.Delete(_product.Code);
        }

        [Then("کالایی با عنوان 'شیر کاله' و کد کالای '1' باید در فهرست کالاها وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Products
                .Should().Contain(_ => _.Code == _product.Code);
            _dataContext.Products
                .Should().Contain(_ => _.Name == _product.Name);
            _dataContext.Products.Should().Contain(_ => _.Id == _product.Id);
        }

        [And("خطایی با عنوان 'کالا با سابقه ی موجودی قابل حذف نیست' باید رخ دهد")]
        public void ThenAnd()
        {
            expected.Should().ThrowExactly<UnableToDeleteProductWithExistingPurchaseBillException>();
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
