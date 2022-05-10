using Shop.Entities;
using Shop.Services.SaleBills.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Persistence.EF.SaleBills
{
    public class EFSaleBillRepository : SaleBillRepository
    {
        private readonly EFDataContext _dataContext;

        public EFSaleBillRepository(EFDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(SaleBill saleBill)
        {
            _dataContext.SaleBills.Add(saleBill);
        }

        public void Delete(SaleBill saleBill)
        {
            _dataContext.SaleBills.Remove(saleBill);
        }

        public SaleBill FindById(int id)
        {
            return _dataContext.SaleBills.Find(id);
        }
    }
}
