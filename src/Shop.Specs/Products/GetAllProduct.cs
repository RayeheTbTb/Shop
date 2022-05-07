using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Shop.Specs.Infrastructure;
using static Shop.Specs.BDDHelper;
using Shop.Persistence.EF;
using Shop.Services.Products.Contracts;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Infrastructure.Application;
using Shop.Services.Products;
using Shop.Persistence.EF.PurchaseBills;
using Shop.Persistence.EF.Products;
using Shop.Entities;
using Shop.Test.Tools;

namespace Shop.Specs.Products
{
    [Scenario("مشاهده کالا")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " کالا را مدیریت کنم",
        InOrderTo = "کالای خود را تعریف کنم"
    )]
    public class GetAllProduct : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        private IList<GetProductDto> expected;
        public GetAllProduct(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10' وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = new ProductBuilder(_category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("درخواست مشاهده فهرست کالا ها را میدهم")]
        public void When()
        {
            expected = _sut.GetAll();
        }

        [Then("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10' باید نمایش داده شود")]
        public void Then()
        {
            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.Id == _product.Id);
            expected.Should().Contain(_ => _.Name == _product.Name);
            expected.Should()
                .Contain(_ => _.InStockCount == _product.InStockCount);
            expected.Should().Contain(_ => _.Code == _product.Code);
            expected.Should().Contain(_ => _.Price == _product.Price);
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
