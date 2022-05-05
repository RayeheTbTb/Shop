using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using static Shop.Specs.BDDHelper;
using Shop.Specs.Infrastructure;
using Shop.Persistence.EF;
using Shop.Services.Products.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Products;
using Shop.Services.Products;
using Shop.Entities;
using Shop.Test.Tools;
using Shop.Services.Products.Exceptions;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Persistence.EF.PurchaseBills;

namespace Shop.Specs.Products
{
    [Scenario("تعریف کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class AddProductWithDuplicateNameInCategory : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        Action expected;

        public AddProductWithDuplicateNameInCategory(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("دسته بندی با عنوان 'لبنیات' وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory("Dairy");
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
        }

        [And("کالایی با عنوان 'شیر کاله' در دسته بندی با عنوان 'لبنیات' وجود دارد")]
        public void GivenAnd()
        {
            _product = ProductFactory.CreateProduct(_category, "Kale Milk");
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("کالایی با عنوان 'شیر کاله' در دسته بندی با عنوان 'لبنیات' تعریف میکنم")]
        public void When()
        {
            var dto = new DefineProductDto
            {
                Name = _product.Name,
                CategoryId = _category.Id,
                Code = 2,
                MinimumInStock = 3
            };
            expected = () => _sut.Add(dto);
        }

        [Then("تنها یک کالا با عنوان 'شیر کاله' در دسته بندی با عنوان 'لبنیات' باید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Products
                .Where(_ => _.Name == _product.Name 
                && _.CategoryId == _category.Id).Should().HaveCount(1);
        }

        [And("خطایی با عنوان 'عنوان کالا در دسته بندی تکراریست' باید رخ دهد")]
        public void ThenAnd()
        {
            expected.Should().ThrowExactly<DuplicateProductNameInCategoryException>();
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

