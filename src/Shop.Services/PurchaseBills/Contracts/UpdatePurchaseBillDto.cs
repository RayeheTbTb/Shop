using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.PurchaseBills.Contracts
{
    public class UpdatePurchaseBillDto
    {
        public string ProductName { get; set; }
        public int ProductCode { get; set; }
        public int Count { get; set; }
        public int WholePrice { get; set; }
        public string SellerName { get; set; }
    }
}
