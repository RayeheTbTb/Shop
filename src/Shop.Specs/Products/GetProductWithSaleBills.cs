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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class GetProductWithSaleBills : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly ProductRepository _repository;
        private readonly PurchaseBillRepository _purchaseBillRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _sut;
        private Category _category;
        private Product _product;
        private GetProductWithSaleBillsDto expected;
        public GetProductWithSaleBills(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFProductRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _purchaseBillRepository = new EFPurchaseBillRepository(_dataContext);
            _sut = new ProductAppService(_repository, _unitOfWork, _purchaseBillRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10'و یک فاکتور فروش وجود دارد")]
        public void Given()
        {
            _category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(_category, _dataContext);
            _product = new ProductBuilder(_category)
                .WithName("Kale Milk").WithCode(1).WithPrice(10000)
                .WithSaleBill().Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [When("درخواست مشاهده فاکتورهای فروش کالا با کد محصول '1' را میدهم")]
        public void When()
        {
            expected = _sut.GetSaleBills(_product.Code);
        }

        [Then("کالایی با عنوان 'شیر کاله'، قیمت '10000تومان'، کد محصول '1'، موجودی '10' و یک فاکتور فروش باید نمایش داده شود")]
        public void Then()
        {
            var saleBills = _product.SaleBills.FirstOrDefault();
            expected.Id.Should().Be(_product.Id);
            expected.Name.Should().Be(_product.Name);
            expected.InStockCount.Should().Be(_product.InStockCount);
            expected.Code.Should().Be(_product.Code);
            expected.Price.Should().Be(_product.Price);
            expected.SaleBills.Should().HaveCount(1);
            expected.SaleBills.Should()
                .Contain(_ => _.CustomerName == saleBills.CustomerName);
            expected.SaleBills.Should().Contain(_ => _.Id == saleBills.Id);
            expected.SaleBills.Should()
                .Contain(_ => _.Count == saleBills.Count);
            expected.SaleBills.Should()
                .Contain(_ => _.Date.Date == saleBills.Date.Date);
            expected.SaleBills.Should()
                .Contain(_ => _.WholePrice == saleBills.WholePrice);
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
