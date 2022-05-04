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
    }
}
