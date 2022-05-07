using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.SaleBills.Contracts
{
    public class GetSaleBillDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int Count { get; set; }
        public int WholePrice { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
    }
}
