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
    [Scenario("اضافه کردن فاکتور فروش")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " فاکتور فروش را مدیریت کنم",
        InOrderTo = "فاکتور فروش خود را تعریف کنم"
    )]
    public class AddSaleBill : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly SaleBillService _sut;
        private Product _product;
        private AddSaleBillDto _dto;
        public AddSaleBill(ConfigurationFixture configuration) : base(configuration)
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
                .WithName("Kale Milk")
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

            _sut.Add(_dto);
        }

        [Then("فاکتور فروش کالایی با کد '1' با تعداد '5' در تاریخ '21 / 02 / 1400' در فهرست فاکتور فروش کالا باید وجود داشته باشد")]
        public void Then()
        {
            var expected = _dataContext.SaleBills.FirstOrDefault();
            expected.CustomerName.Should().Be(_dto.CustomerName);
            expected.Count.Should().Be(_dto.Count);
            expected.ProductId.Should().Be(_product.Id);
            expected.WholePrice.Should().Be(_dto.WholePrice);
            expected.Date.Date.Should().Be(_dto.Date.Date);
        }

        [And("کالایی با عنوان 'شیر کاله' و کد کالای '1' موجودی '5' عدد در فهرست کالا ها باید وجود داشته باشد")]
        public void ThenAnd()
        {
            var expectedProduct = _dataContext.Products
                .Where(_ => _.Id == _product.Id).FirstOrDefault();
            expectedProduct.InStockCount.Should().Be(5);
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
