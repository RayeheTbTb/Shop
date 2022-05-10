namespace Shop.Services.Products.Contracts
{
    public class DefineProductDto
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int MinimumInStock { get; set; }
        public int CategoryId { get; set; }
    }
}
