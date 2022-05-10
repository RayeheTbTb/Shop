using System;
using Xunit;
using FluentAssertions;
using Shop.Specs.Infrastructure;
using Shop.Persistence.EF;
using Shop.Services.Categories.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Categories;
using Shop.Services.Categories;
using static Shop.Specs.BDDHelper;
using Shop.Test.Tools;
using Shop.Infrastructure.Test;
using Shop.Entities;
using Shop.Services.Categories.Exceptions;

namespace Shop.Specs.Categories
{
    [Scenario("حذف دسته بندی")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " دسته بندی کالا را مدیریت کنم",
        InOrderTo = "در آن کالای خود را تعریف کنم"
    )]
    public class DeleteCategoryWithProduct : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly CategoryRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private Category _category;
        Action expected;
        public DeleteCategoryWithProduct(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFCategoryRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _sut = new CategoryAppService(_repository, _unitOfWork);
        }

        [Given("دسته بندی با عنوان 'لبنیات' در فهرست دسته بندی کالا وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            _dataContext.Manipulate(_ => _.Categories.Add(_category));
        }

        [And("کالایی با عنوان 'شیر کاله' در دسته بندی با عنوان 'لبنیات' وجود دارد")]
        public void GivenAnd()
        {
            var product = new Product
            {
                CategoryId = _category.Id,
                Category = _category,
                Name = "Kale Milk"
            };

            _dataContext.Manipulate(_ => _.Products.Add(product));
        }

        [When("دسته بندی با عنوان 'لبنیات' را حذف میکنم")]
        public void When()
        {
            expected = () => _sut.Delete(_category.Id);
        }

        [Then("دسته بندی با عنوان 'لبنیات'در فهرست دسته بندی کالا باید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Categories.Should()
                .Contain(_ => _.Id == _category.Id 
                    && _.Title == _category.Title);
        }

        [And("خطایی با عنوان 'دسته بندی با کالای موجود قابل حذف نیست ' باید رخ دهد")]
        public void ThenAnd()
        {
            expected.Should()
                .ThrowExactly<UnableToDeleteCategoryWithExistingProductException>();
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
