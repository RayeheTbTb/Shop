using System;

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
