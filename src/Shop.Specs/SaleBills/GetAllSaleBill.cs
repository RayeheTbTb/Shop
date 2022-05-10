using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.SaleBills;
using Shop.Services.Products.Contracts;
using Shop.Services.SaleBills;
using Shop.Services.SaleBills.Contracts;
using Shop.Specs.Infrastructure;
using Shop.Test.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.SaleBills
{
    [Scenario("نمایش فاکتور فروش")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " فاکتور فروش را مدیریت کنم",
        InOrderTo = "فاکتور فروش خود را تعریف کنم"
    )]
    public class GetAllSaleBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly SaleBillService _sut;
        private Product _product;
        private IList<GetSaleBillDto> expected;
        public GetAllSaleBill(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFSaleBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new SaleBillAppService(_repository, _unitOfWork,
                _productRepository);
        }

        [Given("فاکتور فروشی به تاریخ '01 / 01 / 1400' به نام 'خریدار' برای کالای با عنوان 'شیر کاله' به تعداد '5' با مجموع قیمت '5000 تومان' در فهرست فاکتور فروش کالا وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithInStockCount(5)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 5000).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
            
        }

        [When("درخواست مشاهده فهرست فاکتورهای فروش کالا ها را میدهم")]
        public void When()
        {
            expected = _sut.GetAll();
        }

        [Then("فاکتور فروشی به تاریخ '01 / 01 / 1400' به نام 'خریدار' برای کالای با عنوان 'شیر کاله' به تعداد '5' با مجموع قیمت '5000 تومان' باید نمایش داده شود")]
        public void Then()
        {
            var purchasebill = _product.SaleBills.First();
            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.ProductId == _product.Id);
            expected.Should().Contain(_ => _.CustomerName == purchasebill.CustomerName);
            expected.Should().Contain(_ => _.Date.Date == purchasebill.Date.Date);
            expected.Should().Contain(_ => _.Count == purchasebill.Count);
            expected.Should().Contain(_ => _.WholePrice == purchasebill.WholePrice);
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
