using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.SaleBills.Contracts
{
    public class UpdateSaleBillDto
    {
        public string ProductName { get; set; }
        public int Count { get; set; }
        public int WholePrice { get; set; }
        public string CustomerName { get; set; }
        public int ProductCode { get; set; }
    }
}
