using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities
{
    public class PurchaseBill : Bill
    {
        public int Id { get; set; }
        public string SellerName { get; set; }
    }
}
