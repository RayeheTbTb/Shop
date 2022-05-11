using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Services.Products;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Specs.Infrastructure;
using Shop.Test.Tools;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.Products
{
    [Scenario("مشاهده کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class GetProduct : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Product _product;
        GetProductDto expected;
        public GetProduct(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository =
                new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork,
                _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10' وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1).WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("درخواست مشاهده کالا را میدهم")]
        public void When()
        {
            expected = _sut.Get(_product.Code);
        }

        [Then("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10' باید نمایش داده شود")]
        public void Then()
        {
            expected.Id.Should().Be(_product.Id);
            expected.MinimumInStock.Should().Be(_product.MinimumInStock);
            expected.CategoryId.Should().Be(_product.CategoryId);
            expected.Name.Should().Be(_product.Name);
            expected.InStockCount.Should().Be( _product.InStockCount);
            expected.Code.Should().Be(_product.Code);
            expected.Price.Should().Be(_product.Price);
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
