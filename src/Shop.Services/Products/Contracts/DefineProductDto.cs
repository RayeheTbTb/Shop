using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
