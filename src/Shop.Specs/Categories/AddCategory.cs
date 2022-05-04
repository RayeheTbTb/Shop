using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Services.Categories.Contracts;
using Shop.Specs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.Categories
{
    [Scenario("تعریف دسته بندی")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " دسته بندی کالا را مدیریت کنم",
        InOrderTo = "در آن کالای خود را تعریف کنم"
    )]
    public class AddCategory : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private readonly CategoryRepository _repository;
        private AddCategoryDto _dto;

        public AddCategory(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
        }

        [Given("هیچ دسته بندی در فهرست دسته بندی کالا وجود ندارد")]
        public void Given()
        {

        }

        [When("دسته بندی با عنوان 'لبنیات ' تعریف میکنم")]
        public void When()
        {
            _dto = new AddCategoryDto
            {
                Title = "Dairy"
            };
            _sut.Add(_dto);
        }

        [Then("دسته بندی با عنوان 'لبنیات' در فهرست دسته بندی کالا باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.Categories.FirstOrDefault();
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
