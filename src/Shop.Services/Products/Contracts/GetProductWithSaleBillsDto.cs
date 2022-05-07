using Shop.Services.SaleBills.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Products.Contracts
{
    public class GetProductWithSaleBillsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int Price { get; set; }
        public int InStockCount { get; set; }
        public IList<GetSaleBillDto> SaleBills { get; set; }
    }
}
