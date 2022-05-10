using Microsoft.AspNetCore.Mvc;
using Shop.Services.Products.Contracts;
using System.Collections.Generic;

namespace Shop.RestAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : Controller
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpPost]
        public void Add(DefineProductDto dto)
        {
            _service.Add(dto);
        }

        [HttpPut("/addStock")]
        public void AddToStock(AddProductToStockDto dto)
        {
            _service.AddToStock(dto);
        }

        [HttpPut("{code}")]
        public void Update(int code, UpdateProductDto dto)
        {
            _service.Update(code, dto);
        }

        [HttpGet]
        public IList<GetProductDto> GetAll()
        {
            return _service.GetAll();
        }

        [HttpGet("{code}")]
        public GetProductDto Get(int code)
        {
            return _service.Get(code);
        }

        [HttpGet("/purchasebill/{code}")]
        public GetProductWithPurchaseBillsDto GetPurchaseBills(int code)
        {
            return _service.GetPurchaseBills(code);
        }

        [HttpGet("/salebill/{code}")]
        public GetProductWithSaleBillsDto GetsaleBills(int code)
        {
            return _service.GetSaleBills(code);
        }

        [HttpDelete("{code}")]
        public void Delete(int code)
        {
            _service.Delete(code);
        }

    }
}
