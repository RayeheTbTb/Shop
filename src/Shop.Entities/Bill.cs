using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities
{
    public class Bill
    {
        public int Count { get; set; }
        public int WholePrice { get; set; }
        public DateTime Date { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
