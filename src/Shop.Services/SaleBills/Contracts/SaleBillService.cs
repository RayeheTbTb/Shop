using Shop.Infrastructure.Application;
using System.Collections.Generic;

namespace Shop.Services.SaleBills.Contracts
{
    public interface SaleBillService : Service
    {
        void Add(AddSaleBillDto dto);
        void Delete(int id);
        void Update(int id, UpdateSaleBillDto dto);
        IList<GetSaleBillDto> GetAll();
        GetSaleBillDto Get(int id);
    }
}
