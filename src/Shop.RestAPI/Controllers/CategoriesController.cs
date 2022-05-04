using Microsoft.AspNetCore.Mvc;
using Shop.Services.Categories.Contracts;
using Shop.Services.Products.Contracts;
using System.Collections;
using System.Collections.Generic;

namespace Shop.RestAPI.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : Controller
    {
        private readonly CategoryService _service;

        public CategoriesController(CategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public void Add(AddCategoryDto dto)
        {
            _service.Add(dto);
        }

        [HttpPut("{id}")]
        public void Update(int id, UpdateCategoryDto dto)
        {
            _service.Update(id, dto);
        }

        [HttpGet]
        public IList<GetCategoryDto> GetAll()
        {
            return _service.GetAll();
        }

        [HttpGet("{id}")]
        public IList<GetProductDto> GetProducts(int id)
        {
            return _service.GetProducts(id);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _service.Delete(id);
        }
    }
}
