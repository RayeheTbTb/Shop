using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Products.Contracts
{
    public class AddProductToStockDto
    {
        public int Code { get; set; }
        public int WholePrice { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string SellerName { get; set; }
    }
}
