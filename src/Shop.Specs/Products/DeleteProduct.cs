using Shop.Specs.Infrastructure;
using Xunit;
using FluentAssertions;
using static Shop.Specs.BDDHelper;
using Shop.Persistence.EF;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products;
using Shop.Entities;
using Shop.Test.Tools;

namespace Shop.Specs.Products
{
    [Scenario("حذف کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class DeleteProduct : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        public DeleteProduct(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = 
                new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, 
                _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله ' و کد کالای '1' تعریف شده است")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = new ProductBuilder(_category)
                .WithName("Kale Milk").Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("کالایی با عنوان 'شیر کاله ' و کد کالای '1' را حذف میکنم")]
        public void When()
        {
            _sut.Delete(_product.Code);
        }

        [Then("کالایی با عنوان 'شیر کاله' و کد کالای '1' نباید در فهرست کالاها وجود داشته باشد")]
        public void Then()
        {
            _dataContext.Products.Should()
                .NotContain(_ => _.Code == _product.Code);
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
