using System;

namespace Shop.Services.SaleBills.Contracts
{
    public class AddSaleBillDto
    {
        public int Count { get; set; }
        public int WholePrice { get; set; }
        public DateTime Date { get; set; }
        public int ProductCode { get; set; }
        public string CustomerName { get; set; }
    }
}
