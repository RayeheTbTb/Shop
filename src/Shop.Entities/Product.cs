using System.Collections.Generic;

namespace Shop.Entities
{
    public class Product
    {
        public Product()
        {
            SaleBills = new HashSet<SaleBill>();
            PurchaseBills = new HashSet<PurchaseBill>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int Price { get; set; }
        public int MinimumInStock { get; set; }
        public int InStockCount { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public HashSet<SaleBill> SaleBills { get; set; }

        public HashSet<PurchaseBill> PurchaseBills { get; set; }
    }
}
