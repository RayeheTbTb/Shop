using System.Linq;
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
    public class AddProduct : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private DefineProductDto _dto;
        private Category _category;
        public AddProduct(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = 
                new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, 
                _purchaseBillRepository);
        }

        [Given("دسته بندی با عنوان 'لبنیات' در فهرست دسته بندی کالا وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory("Dairy");
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
        }

        [And("هیچ کالایی در فهرست دسته بندی با عنوان 'لبنیات' وجود ندارد")]
        public void GivenAnd()
        {
            
        }

        [When("کالایی با عنوان 'شیر کاله'، کد محصول '1' و حداقل موجودی '3' در دسته بندی تعریف میکنم")]
        public void When()
        {
            _dto = new DefineProductDto
            {
                Name = "Kale Milk",
                Code = 1,
                MinimumInStock = 3,
                CategoryId = _category.Id
            };
            _sut.Add(_dto);
        }

        [Then("کالایی با عنوان 'شیر کاله'، کد محصول '1' و حداقل موجودی '3' دردسته بندی با عنوان 'لبنیات' باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.Products.FirstOrDefault();
            expected.Name.Should().Be(_dto.Name);
            expected.Code.Should().Be(_dto.Code);
            expected.MinimumInStock.Should().Be(_dto.MinimumInStock);
            expected.CategoryId.Should().Be(_category.Id);
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
