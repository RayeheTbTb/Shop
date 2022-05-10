using Shop.Entities;
using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.SaleBills.Contracts
{
    public interface SaleBillRepository : Repository
    {
        void Add(SaleBill saleBill);
        SaleBill FindById(int id);
        void Delete(SaleBill saleBill);
        IList<GetSaleBillDto> GetAll();
        bool IsExistBill(int id);
        GetSaleBillDto Get(int id);
    }
}
