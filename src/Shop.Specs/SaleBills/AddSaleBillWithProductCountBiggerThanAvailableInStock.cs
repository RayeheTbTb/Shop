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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class AddSaleBillWithProductCountBiggerThanAvailableInStock : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly SaleBillService _sut;
        private Product _product;
        Action expected;
        public AddSaleBillWithProductCountBiggerThanAvailableInStock(ConfigurationFixture configuration) : base(configuration)
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

        [When("فاکتور فروش کالایی برای کالایی به نام 'خریدار' با کد '1' با تعداد '11' با مجموع قیمت '50000 تومان' در تاریخ '21 / 02 / 1400' وارد میکنیم")]
        public void When()
        {
            var dto = new AddSaleBillDto
            {
                Count = 11,
                CustomerName = "Customer",
                ProductCode = _product.Code,
                WholePrice = 50000,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z")
            };

            expected = () => _sut.Add(dto);
        }

        [Then("هیچ فاکتور فروش کالایی در فهرست فاکتور فروش کالا نباید وجود داشته باشد و موجودی کالا نباید تغییر کند")] //
        public void Then()
        {
            _dataContext.SaleBills.Should().HaveCount(0);
            _product.InStockCount.Should().Be(10);
        }

        [And("خطایی با عنوان 'تعداد کالای درخواستی بیشتر از موجودی است' باید نمایش داده شود")]
        public void ThenAnd()
        {
            expected.Should().ThrowExactly<NotEnoughProductInStockException>();
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
