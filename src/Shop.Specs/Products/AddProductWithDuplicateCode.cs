using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Shop.Specs.Infrastructure;
using Shop.Persistence.EF;
using Shop.Services.Products.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Products;
using Shop.Services.Products;
using static Shop.Specs.BDDHelper;
using Shop.Test.Tools;
using Shop.Entities;
using Shop.Services.Products.Exceptions;

namespace Shop.Specs.Products
{
    [Scenario("تعریف کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class AddProductWithDuplicateCode : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        Action expected;
        public AddProductWithDuplicateCode(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork);
        }

        [Given("کالایی با کد کالای '1' در فهرست کالاها وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = ProductFactory.CreateProduct(_category);
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("کالایی با کد کالای '1' تعریف میکنم")]
        public void When()
        {
            var dto = new DefineProductDto
            {
                Code = _product.Code,
                Name = "name",
                MinimumInStock = 1,
                CategoryId = _category.Id
            };
            expected = () => _sut.Add(dto);

        }

        [Then("تنها یک کالا با کد کالای '1' در فهرست کالا باید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Products
                .Where(_ => _.Code == _product.Code).Should().HaveCount(1);
        }

        [And("خطایی با عنوان 'کد کالا تکراریست' باید رخ دهد")]
        public void ThenAnd()
        {
            expected.Should().ThrowExactly<DuplicateProductCodeException>();
        }

        [Fact]
        public void Run()
        {
            Given();
            When();
            Then();
            ThenAnd();
        }
    }
}
