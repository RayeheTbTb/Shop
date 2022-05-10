using Shop.Entities;
using Shop.Services.SaleBills.Contracts;
using System.Collections.Generic;
using System.Linq;

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

        public GetSaleBillDto Get(int id)
        {
            return _dataContext.SaleBills.Where(_ => _.Id == id)
                .Select(_ => new GetSaleBillDto
                {
                    Id = _.Id,
                    Count = _.Count,
                    CustomerName = _.CustomerName,
                    Date = _.Date,
                    ProductId = _.ProductId,
                    WholePrice = _.WholePrice
                }).FirstOrDefault();
        }

        public IList<GetSaleBillDto> GetAll()
        {
            return _dataContext.SaleBills.Select(_ => new GetSaleBillDto
            {
                Count = _.Count,
                CustomerName = _.CustomerName,
                Date = _.Date,
                Id = _.Id,
                ProductId = _.ProductId,
                WholePrice = _.WholePrice
            }).ToList();
        }

        

        public bool IsExistBill(int id)
        {
            return _dataContext.SaleBills.Any(_ => _.Id == id);
        }
    }
}
