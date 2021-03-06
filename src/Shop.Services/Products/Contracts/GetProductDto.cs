namespace Shop.Services.Products.Contracts
{
    public class GetProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int Price { get; set; }
        public int InStockCount { get; set; }
        public int CategoryId { get; set; }
        public int MinimumInStock { get; set; }
    }
}
