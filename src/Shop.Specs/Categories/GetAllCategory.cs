using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Shop.Specs.Infrastructure;
using Shop.Persistence.EF;
using static Shop.Specs.BDDHelper;
using Shop.Infrastructure.Application;
using Shop.Services.Categories.Contracts;
using Shop.Persistence.EF.Categories;
using Shop.Services.Categories;
using Shop.Entities;
using Shop.Test.Tools;
using Shop.Infrastructure.Test;

namespace Shop.Specs.Categories
{
    [Scenario("نمایش دسته بندی")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " دسته بندی کالا را مدیریت کنم",
        InOrderTo = "در آن کالای خود را تعریف کنم"
    )]
    public class GetAllCategory : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private readonly CategoryRepository _repository;
        private Category _category;
        private IList<GetCategoryDto> expected;

        public GetAllCategory(ConfigurationFixture configuration) : base(configuration)
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

        [When("درخواست مشاهده فهرست دسته بندی کالا را میدهم")]
        public void When()
        {
            expected = _sut.GetAll();
        }

        [Then("دسته بندی با عنوان 'لبنیات' باید نمایش داده شود")]
        public void Then()
        {
            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.Id == _category.Id);
            expected.Should().Contain(_ => _.Title == _category.Title);
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
