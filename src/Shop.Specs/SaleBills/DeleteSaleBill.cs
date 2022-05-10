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
using System.Linq;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.SaleBills
{
    [Scenario("حذف فاکتور فروش")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " فاکتور فروش را مدیریت کنم",
        InOrderTo = "فاکتور فروش خود را تعریف کنم"
    )]
    public class DeleteSaleBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly SaleBillService _sut;
        private Product _product;
        private int _billId;
        public DeleteSaleBill(ConfigurationFixture configuration) : base(configuration)
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
            _billId = _product.SaleBills.First().Id;
        }

        [When("فاکتور فروش را حذف میکنم")]
        public void When()
        {
            _sut.Delete(_billId);
        }

        [Then("فاکتور فروشی به تاریخ '01 / 01 / 1400' به نام 'خریدار' برای کالای با عنوان 'شیر کاله' به تعداد '5' با مجموع قیمت '5000 تومان' در فهرست فاکتور فروش کالا نباید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.SaleBills.Should().NotContain(_ => _.Id == _billId);
        }

        [And("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '10' عدد در فهرست کالا ها وجود دارد")] //
        public void ThenAnd()
        {
            _dataContext.Products.FirstOrDefault(_ => _.Id == _product.Id)
               .InStockCount.Should().Be(10);
        }

        [Fact]
        public void Run()
        {
            Given();
            When();
            Then();
            ThenAnd();
        }
    }
}
