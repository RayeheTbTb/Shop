using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities
{
    public class SaleBill : Bill
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
    }
}
