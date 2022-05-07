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
using Shop.Services.Categories.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Categories;
using Shop.Services.Categories;
using Shop.Test.Tools;
using Shop.Entities;
using Shop.Infrastructure.Test;
using Shop.Services.Products.Contracts;

namespace Shop.Specs.Categories
{
    [Scenario("نمایش دسته بندی")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " دسته بندی کالا را مدیریت کنم",
        InOrderTo = "در آن کالای خود را تعریف کنم"
    )]
    public class GetCategoryWithProducts : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private readonly CategoryRepository _repository;
        private Category _category;
        private Product _product;
        private IList<GetProductDto> expected;
        public GetCategoryWithProducts(ConfigurationFixture configuration) : base(configuration)
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

        [And("کالایی با عنوان 'شیر کاله'و کد کالای '1' در دسته بندی با عنوان 'لبنیات' وجود دارد")]
        public void GivenAnd()
        {
            _product = new ProductBuilder(_category)
                .WithName("Kale Milk").WithCode(1).Build();
            _dataContext.Manipulate(_ => _.Products.Add(_product));
        }

        [When("درخواست مشاهده کالاهای دسته بندی با عنوان 'لبنیات' را میدهم")]
        public void When()
        {
            expected = _sut.GetProducts(_category.Id);
        }

        [Then("کالایی با عنوان 'شیر کاله'و کد کالای '1' باید نمایش داده شود")]
        public void Then()
        {
            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.Name == _product.Name);
            expected.Should().Contain(_ => _.Id == _product.Id);
            expected.Should().Contain(_ => _.Code == _product.Code);
            expected.Should().Contain(_ => _.Price == _product.Price);
            expected.Should().Contain(_ => _.InStockCount == _product.InStockCount);
        }

        [Fact]
        public void Run()
        {
            Given();
            GivenAnd();
            When();
            Then();
        }
    }
}
