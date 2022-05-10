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
using System.Linq;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.Categories
{
    [Scenario("ویرایش دسته بندی")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " دسته بندی کالا را مدیریت کنم",
        InOrderTo = "در آن کالای خود را تعریف کنم"
    )]
    public class UpdateCategory : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly CategoryRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private Category _category;
        private UpdateCategoryDto _dto;
        public UpdateCategory(ConfigurationFixture configuration) : base(configuration)
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

        [When("نام دسته بندی با عنوان 'لبنیات' را به 'نوشیدنی ها' تغییر میدهم")]
        public void When()
        {
            _dto = new UpdateCategoryDto
            {
                Title = "Drinks"
            };
            _sut.Update(_category.Id, _dto);
        }

        [Then("دسته بندی با عنوان 'نوشیدنی ها'در فهرست دسته بندی کالا باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.Categories
                .FirstOrDefault(_ => _.Id == _category.Id);
            expected.Title.Should().Be(_dto.Title);
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
