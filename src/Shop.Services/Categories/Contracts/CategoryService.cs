using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Categories.Contracts
{
    public interface CategoryService : Service
    {
        void Add(AddCategoryDto dto);
    }
}
