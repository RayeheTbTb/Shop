using FluentAssertions;
using Shop.Entities;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Categories;
using Shop.Services.Categories;
using Shop.Services.Categories.Contracts;
using Shop.Services.Categories.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Shop.Services.Test.Unit.Categories
{
    public class CategoryServiceTests
    {
        private readonly EFDataContext _dataContext;
        private readonly CategoryService _sut;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryRepository _repository;

        public CategoryServiceTests()
        {
            _dataContext = new EFInMemoryDatabase().CreateDataContext<EFDataContext>();
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _repository = new EFCategoryRepository(_dataContext);
            _sut = new CategoryAppService(_repository, _unitOfWork);
        }

        [Fact]
        public void Add_adds_category_properly()
        {
            var dto = GenerateAddCategoryDto("dummy");

            _sut.Add(dto);

            _dataContext.Categories.Should().Contain(_ => _.Title == dto.Title);
        }

        [Fact]
        public void Add_throws_DuplicateCategoryException_when_given_category_title_already_exists()
        {
            var category = GenerateCategory("dummy");
            AddCategoryToDatabase(category);
            var dto = GenerateAddCategoryDto("dummy");

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<DuplicateCategoryTitleException>();
        }

        [Fact]
        public void Update_updates_category_properly()
        {
            var category = GenerateCategory("dummy");
            AddCategoryToDatabase(category);
            var dto = GenerateUpdateCategoryDto("updatDummy");

            _sut.Update(category.Id, dto);

            var expected = _dataContext.Categories.FirstOrDefault(_ => _.Id == category.Id);
            expected.Title.Should().Be(dto.Title);
        }

        [Theory]
        [InlineData(4)]
        public void Update_throws_CategoryNotFoundException_when_category_with_given_id_does_not_exist(int fakeCategoryId)
        {
            var dto = GenerateUpdateCategoryDto("updateDummy");

            Action expected = () => _sut.Update(fakeCategoryId, dto);

            expected.Should().ThrowExactly<CategoryNotFoundException>();
        }

        [Fact]
        public void Delete_deletes_category_propely()
        {
            var category = GenerateCategory("dummy");
            AddCategoryToDatabase(category);

            _sut.Delete(category.Id);

            _dataContext.Categories.Should()
                .NotContain(_ => _.Id == category.Id
                    && _.Title == category.Title);
        }

        [Theory]
        [InlineData(4)]
        public void Delete_throws_CategoryNotFoundException_when_category_with_given_id_does_not_exist(int fakeCategoryId)
        {
            Action expected = () => _sut.Delete(fakeCategoryId);

            expected.Should().ThrowExactly<CategoryNotFoundException>();
        }

        [Fact]
        public void Delete_throws_UnableToDeleteCategoryWithProductException_when_given_category_has_at_least_one_product()
        {
            var category = GenerateCategory("dummy");
            AddCategoryToDatabase(category);
            var product = GenerateProduct(category);
            AddProductToDatabase(product);

            Action expected = () => _sut.Delete(category.Id);

            expected.Should().ThrowExactly<UnableToDeleteCategoryWithProductException>();
        }

        [Fact]
        public void GetAll_returns_all_categories()
        {
            var category = GenerateCategory("dummy");
            AddCategoryToDatabase(category);

            var expected = _sut.GetAll();

            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.Id == category.Id);
            expected.Should().Contain(_ => _.Title == category.Title);
        }

        [Fact]
        public void GetProducts_returns_all_products_in_category_with_given_id()
        {
            var category = GenerateCategory("dummy");
            AddCategoryToDatabase(category);
            var product = GenerateProduct(category);
            AddProductToDatabase(product);

            var expected = _sut.GetProducts(category.Id);

            expected.Should().HaveCount(1);
            expected.Should().Contain(_ => _.Name == product.Name);
            expected.Should().Contain(_ => _.Id == product.Id);
            expected.Should().Contain(_ => _.Code == product.Code);
            expected.Should().Contain(_ => _.Price == product.Price);
            expected.Should().Contain(_ => _.InStockCount == product.InStockCount);
        }

        private static AddCategoryDto GenerateAddCategoryDto(string title)
        {
            return new AddCategoryDto
            {
                Title = title
            };
        }

        private static Category GenerateCategory(string title)
        {
            return new Category
            {
                Title = title
            };
        }

        private void AddCategoryToDatabase(Category category)
        {
            _dataContext.Manipulate(_ => _.Categories.Add(category));
        }

        private static UpdateCategoryDto GenerateUpdateCategoryDto(string title)
        {
            return new UpdateCategoryDto
            {
                Title = title
            };
        }
        private static Product GenerateProduct(Category category)
        {
            return new Product
            {
                Name = "dummy",
                Code = 1,
                Category = category,
                CategoryId = category.Id,
                Price = 1000,
                InStockCount = 5
            };
        }

        private void AddProductToDatabase(Product product)
        {
            _dataContext.Manipulate(_ => _.Products.Add(product));
        }
    }
}
