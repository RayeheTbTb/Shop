using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.SaleBills.Contracts
{
    public interface SaleBillService : Service
    {
        void Add(AddSaleBillDto dto);
        void Delete(int id);
    }
}
