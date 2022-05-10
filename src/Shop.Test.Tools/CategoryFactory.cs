using Shop.Entities;
using Shop.Infrastructure.Test;
using Shop.Persistence.EF;

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

        public static void AddCategoryToDatabase(Category category, EFDataContext datacontext)
        {
            datacontext.Manipulate(_ => _.Categories.Add(category));
        }
    }
}
