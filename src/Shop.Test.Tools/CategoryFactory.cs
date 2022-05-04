using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Test.Tools
{
    public static class CategoryFactory
    {
        public static Category CreateCategory()
        {
            return new Category
            {
                Title = "Dairy"
            };
        }

        public static Category CreateCategory(string title)
        {
            return new Category
            {
                Title = title
            };
        }
    }
}
