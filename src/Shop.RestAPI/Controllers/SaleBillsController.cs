using Microsoft.AspNetCore.Mvc;
using Shop.Services.PurchaseBills.Contracts;
using Shop.Services.SaleBills.Contracts;
using System.Collections.Generic;

namespace Shop.RestAPI.Controllers
{
    [ApiController]
    [Route("api/salebills")]
    public class SaleBillsController : Controller
    {
        private readonly SaleBillService _service;

        public SaleBillsController(SaleBillService service)
        {
            _service = service;
        }

        [HttpPost]
        public void Add(AddSaleBillDto dto)
        {
            _service.Add(dto);
        }

        [HttpPut("{id}")]
        public void Update(int id, UpdateSaleBillDto dto)
        {
            _service.Update(id, dto);
        }

        [HttpGet]
        public IList<GetSaleBillDto> GetAll()
        {
            return _service.GetAll();
        }

        [HttpGet("{id}")]
        public GetSaleBillDto Get(int id)
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
