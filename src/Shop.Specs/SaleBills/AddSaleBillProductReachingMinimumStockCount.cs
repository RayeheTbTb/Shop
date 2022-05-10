using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Products;
using Shop.Persistence.EF.SaleBills;
using Shop.Services.Products.Contracts;
using Shop.Services.SaleBills;
using Shop.Services.SaleBills.Contracts;
using Shop.Services.SaleBills.Exceptions;
using Shop.Specs.Infrastructure;
using Shop.Test.Tools;
using System;
using System.Linq;
using Xunit;
using static Shop.Specs.BDDHelper;

namespace Shop.Specs.SaleBills
{
    [Scenario("تعریف  فاکتور فروش")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " فاکتور فروش را مدیریت کنم",
        InOrderTo = "فاکتور فروش خود را تعریف کنم"
    )]
    public class AddSaleBillProductReachingMinimumStockCount : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly SaleBillService _sut;
        private Product _product;
        private AddSaleBillDto _dto;
        Action expected;
        public AddSaleBillProductReachingMinimumStockCount(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFSaleBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new SaleBillAppService(_repository, _unitOfWork,
                _productRepository);
        }

        [Given("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '10' عدد در فهرست کالا ها وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk").WithMinimumInStock(6)
                .WithCode(1).WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
        }

        [And("هیچ فاکتور فروش کالایی در فهرست فاکتور فروش کالا وجود ندارد")]
        public void GivenAnd()
        {

        }

        [When("فاکتور فروش کالایی برای کالایی به نام 'خریدار' با کد '1' با تعداد '5' با مجموع قیمت '5000 تومان' در تاریخ '21 / 02 / 1400' وارد میکنیم")]
        public void When()
        {
            _dto = new AddSaleBillDto
            {
                Count = 5,
                CustomerName = "Customer",
                ProductCode = _product.Code,
                WholePrice = 5000,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z")
            };

            expected = () => _sut.Add(_dto);
        }

        [Then("هیچ فاکتور فروش کالایی در فهرست فاکتور فروش کالا نباید وجود داشته باشد")]
        public void Then()
        {
            _dataContext.SaleBills.Should().HaveCount(0);
        }

        [And("خطایی با عنوان 'تعداد کالا به حداقل موجودی رسیده است ' باید رخ دهد")]
        public void ThenAnd()
        {
            expected.Should().ThrowExactly<ProductReachedMinimumInStockCountException>();

        }

        [Fact]
        public void Run()
        {
            Given();
            GivenAnd();
            When();
            Then();
            ThenAnd();
        }
    }
}
