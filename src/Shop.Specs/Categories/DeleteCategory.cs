using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Categories;
using Shop.Services.Categories;
using Shop.Services.Categories.Contracts;
using Shop.Specs.Infrastructure;
using Shop.Test.Tools;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.Categories
{
    [Scenario("حذف دسته بندی")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " دسته بندی کالا را مدیریت کنم",
        InOrderTo = "در آن کالای خود را تعریف کنم"
    )]
    public class DeleteCategory : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly CategoryRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private Category _category;
        public DeleteCategory(ConfigurationFixture configuration) : base(configuration)
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

        [When("دسته بندی با عنوان 'لبنیات' را حذف میکنم")]
        public void When()
        {
            _sut.Delete(_category.Id);
        }

        [Then("دسته بندی با عنوان 'لبنیات'در فهرست دسته بندی کالا نباید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Categories.Should()
                .NotContain(_ => _.Id == _category.Id 
                    && _.Title == _category.Title);
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
