using Microsoft.AspNetCore.Mvc;
using Shop.Services.PurchaseBills.Contracts;
using System.Collections.Generic;

namespace Shop.RestAPI.Controllers
{
    [ApiController]
    [Route("api/purchasebills")]
    public class PurchaseBillsController : Controller
    {
        private readonly PurchaseBillService _service;

        public PurchaseBillsController(PurchaseBillService service)
        {
            _service = service;
        }

        [HttpPost]
        public void Add(AddPurchaseBillDto dto)
        {
            _service.Add(dto);
        }

        [HttpPut("{id}")]
        public void Update(int id, UpdatePurchaseBillDto dto)
        {
            _service.Update(id, dto);
        }

        [HttpGet]
        public IList<GetPurchaseBillDto> GetAll()
        {
            return _service.GetAll();
        }

        [HttpGet("{id}")]
        public GetPurchaseBillDto Get(int id)
        {
            return _service.Get(id);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _service.Delete(id);
        }

    }
}
