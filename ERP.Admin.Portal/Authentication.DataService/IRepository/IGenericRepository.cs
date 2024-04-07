using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataService.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetBy(Guid id);

        Task<bool> Add(T entity);

        Task<bool> Updated(T entity);

        Task<bool> Delete(Guid id);
    }
}
