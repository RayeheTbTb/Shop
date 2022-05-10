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
