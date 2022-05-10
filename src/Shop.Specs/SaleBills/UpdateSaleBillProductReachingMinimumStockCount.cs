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
    [Scenario("ویرایش  فاکتور فروش")]
    [Feature("",
        AsA = "فروشنده",
        IWantTo = " فاکتور فروش را مدیریت کنم",
        InOrderTo = "فاکتور فروش خود را تعریف کنم"
    )]
    public class UpdateSaleBillProductReachingMinimumStockCount : EFDataContextDatabaseFixture
    {
        private readonly EFDataContext _dataContext;
        private readonly SaleBillRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepository;
        private readonly SaleBillService _sut;
        private Product _product;
        private Product _product2;
        private UpdateSaleBillDto _dto;
        private int _billId;
        Action expected;
        public UpdateSaleBillProductReachingMinimumStockCount(ConfigurationFixture configuration) : base(configuration)
        {
            _dataContext = CreateDataContext();
            _repository = new EFSaleBillRepository(_dataContext);
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _productRepository = new EFProductRepository(_dataContext);
            _sut = new SaleBillAppService(_repository, _unitOfWork,
                _productRepository);
        }

        [Given("فاکتور فروشی به تاریخ '01 / 01 / 1400' به نام 'خریدار' برای کالای با عنوان 'شیر کاله' به تعداد '10' با مجموع قیمت '10000 تومان' در فهرست فاکتور فروش کالا وجود دارد")]
        public void Given()
        {
            var category = CategoryFactory.CreateCategory();
            CategoryFactory.AddCategoryToDatabase(category, _dataContext);
            _product = new ProductBuilder(category)
                .WithName("Kale Milk").WithCode(1)
                .WithInStockCount(20)
                .WithSaleBill("Customer", 5,
                DateTime.Parse("2022-04-27T05:22:05.264Z"), 10000).Build();
            ProductFactory.AddProductToDatabase(_product, _dataContext);
            _product2 = new ProductBuilder(category)
                .WithName("Kale Yogurt").WithCode(2)
                .WithMinimumInStock(6)
                .WithInStockCount(10).Build();
            ProductFactory.AddProductToDatabase(_product2, _dataContext);
            _billId = _product.SaleBills.First().Id;
        }

        [When("نام را به 'خریدار2'، عنوان کالا را به 'ماست کاله' تعداد را به '5' و مجموع قیمت را به '5000 تومان' تغییر میدهم")]
        public void When()
        {
            _dto = new UpdateSaleBillDto
            {
                ProductName = _product2.Name,
                Count = 5,
                WholePrice = 5000,
                CustomerName = "Seller2",
                ProductCode = _product2.Code
            };

            expected = () => _sut.Update(_billId, _dto);
        }

        [Then("فاکتور فروشی به تاریخ '01 / 01 / 1400' به نام 'خریدار' برای کالای با عنوان 'شیر کاله' به تعداد '10' با مجموع قیمت '10000 تومان' در فهرست فاکتور فروش کالا باید وجود داشته باشد")]
        public void Then()
        {
            var expectedSaleBill = _dataContext.SaleBills
                .FirstOrDefault(_ => _.Id == _billId);
            var saleBill = _product.SaleBills.First();

            expectedSaleBill.ProductId.Should().Be(_product.Id);
            expectedSaleBill.CustomerName.Should().Be(saleBill.CustomerName);
            expectedSaleBill.Product.Name.Should().Be(_product.Name);
            expectedSaleBill.Count.Should().Be(saleBill.Count);
            expectedSaleBill.WholePrice.Should().Be(saleBill.WholePrice);
            _product2.SaleBills.Should().NotContain(_ => _.Id == _billId);
            _product.InStockCount.Should().Be(20);
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
            When();
            Then();
            ThenAnd();
        }
    }
}
