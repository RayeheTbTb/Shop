using Shop.Specs.Infrastructure;
using System.Linq;
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
    [Scenario("ویرایش کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class UpdateProduct : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        private UpdateProductDto _dto;
        public UpdateProduct(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = 
                new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, 
                _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله'، کد محصول '1'، حداقل موجودی '1' وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = new ProductBuilder(_category)
                .WithName("Kale Milk").Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("عنوان کالا را به 'شیر رامک' و حداقل موجودی را به '3' تغییر میدهم")]
        public void When()
        {
            _dto = new UpdateProductDto
            {
                Name = "Ramak Milk",
                MinimumInStock = 3
            };

            _sut.Update(_product.Code, _dto);
        }

        [Then("کالایی با عنوان 'شیر رامک'، کد محصول '1'، حداقل موجودی '3' باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.Products
                .FirstOrDefault(_ => _.Code == _product.Code);
            expected.Name.Should().Be(_dto.Name);
            expected.MinimumInStock.Should().Be(_dto.MinimumInStock);
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
