using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Test.Tools
{
    public class ProductBuilder
    {
        private Product product;

        public ProductBuilder(Category category)
        {
            product = new Product
            {
                Name = "dummy",
                Code = 1,
                MinimumInStock = 1,
                InStockCount = 10,
                Price = 10000,
                Category = category,
                CategoryId = category.Id
            };
        }

        public ProductBuilder WithName(string name)
        {
            product.Name = name;
            return this;
        }

        public ProductBuilder WithCode(int code)
        {
            product.Code = code;
            return this;
        }

        public ProductBuilder WithMinimumInStock(int minimumInStock)
        {
            product.MinimumInStock = minimumInStock;
            return this;
        }

        public ProductBuilder WithPrice(int price)
        {
            product.Price = price;
            return this;
        }

        public ProductBuilder WithPurchaseBill()
        {
            product.PurchaseBills.Add(new PurchaseBill
            {
                Product = product,
                ProductId = product.Id,
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                SellerName = "dummyName",
                WholePrice = 10000,
            });
            return this;
        }

        public ProductBuilder WithPurchaseBill(string name, int count, DateTime date, int wholePrice)
        {
            product.PurchaseBills.Add(new PurchaseBill
            {
                Product = product,
                ProductId = product.Id,
                Count = count,
                Date = date,
                SellerName = name,
                WholePrice = wholePrice,
            });
            return this;
        }

        public ProductBuilder WithSaleBill()
        {
            product.SaleBills.Add(new SaleBill
            {
                Product = product,
                ProductId = product.Id,
                Count = 10,
                Date = DateTime.Parse("2022-04-27T05:22:05.264Z"),
                CustomerName = "dummyName",
                WholePrice = 10000,
            });
            return this;
        }

        public ProductBuilder WithSaleBill(string name, int count, DateTime date, int wholePrice)
        {
            product.SaleBills.Add(new SaleBill
            {
                Product = product,
                ProductId = product.Id,
                Count = count,
                Date = date,
                CustomerName = name,
                WholePrice = wholePrice,
            });
            return this;
        }

        public Product Build()
        {
            return product;
        }
    }
}
