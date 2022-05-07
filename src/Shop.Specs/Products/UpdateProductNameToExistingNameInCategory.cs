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
using Shop.Services.PurchaseBills.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products;
using Shop.Entities;
using Shop.Test.Tools;
using Shop.Services.Products.Exceptions;

namespace Shop.Specs.Products
{
    [Scenario("ویرایش کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class UpdateProductNameToExistingNameInCategory : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        private Product _product2;
        private UpdateProductDto _dto;
        Action expected;
        public UpdateProductNameToExistingNameInCategory(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله' در دسته بندی با عنوان 'لبنیات' وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = ProductFactory.CreateProduct(_category, "Kale Milk", 1);
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [And("کالایی با عنوان 'ماست کاله' در دسته بندی با عنوان 'لبنیات' وجود دارد")]
        public void GivenAnd()
        {
            _product2 = ProductFactory.CreateProduct(_category, "Kale Yogurt", 2);
            ProductFactory.AddProductToDatabase(_product2, _dataContext);
        }

        [When("نام کالا با عنوان 'ماست کاله' را به 'شیر کاله' تغییر میدهم")]
        public void When()
        {
            _dto = new UpdateProductDto
            {
                Name = "Kale Milk",
                MinimumInStock = 10
            };
            expected = () => _sut.Update(_product2.Code, _dto);
        }

        [Then("تنها یک کالا با عنوان 'شیر کاله' در دسته بندی با عنوان 'لبنیات' باید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Products
                .Where(_ => _.Name == _product.Name 
                && _.Category.Title == _category.Title).Should().HaveCount(1);
        }

        [And("خطایی با عنوان 'عنوان کالا در دسته بندی تکراریست' باید رخ دهد")]
        public void ThenAnd()
        {
            expected.Should()
                .ThrowExactly<DuplicateProductNameInCategoryException>();
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
