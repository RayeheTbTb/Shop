using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Products.Contracts
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public int MinimumInStock { get; set; }
    }
}
