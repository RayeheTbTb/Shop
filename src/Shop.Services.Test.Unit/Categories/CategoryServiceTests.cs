using FluentAssertions;
using Shop.Infrastructure.Application;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using Shop.Persistence.EF.Categories;
using Shop.Services.Categories;
using Shop.Services.Categories.Contracts;
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

        private static AddCategoryDto GenerateAddCategoryDto(string title)
        {
            return new AddCategoryDto
            {
                Title = title
            };
        }
    }
}
