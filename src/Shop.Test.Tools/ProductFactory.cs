using Shop.Entities;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Test.Tools
{
    public class ProductFactory
    {
        public static Product CreateProduct(Category category)
        {
            return new Product
            {
                Name = "Kale Milk",
                Code = 1,
                MinimumInStock = 1,
                CategoryId = category.Id
            };
        }

        public static Product CreateProduct(Category category, string name)
        {
            return new Product
            {
                Name = name,
                Code = 1,
                MinimumInStock = 1,
                CategoryId = category.Id
            };
        }

        public static void AddProductToDatabase(Product product, EFDataContext datacontext)
        {
            datacontext.Manipulate(_ => _.Products.Add(product));
        }
    }
}
