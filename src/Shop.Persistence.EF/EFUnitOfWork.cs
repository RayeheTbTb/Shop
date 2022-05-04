using Shop.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Persistence.EF
{
    public class EFUnitOfWork : UnitOfWork
    {
        private readonly EFDataContext _dataContext;
        public EFUnitOfWork(EFDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public void Commit()
        {
            _dataContext.SaveChanges();
        }
    }
}
