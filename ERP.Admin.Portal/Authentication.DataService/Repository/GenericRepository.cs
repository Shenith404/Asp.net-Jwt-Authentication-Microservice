using Authentication.DataService.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataService.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        public readonly ILogger _logger;
        protected AppDbContext _context;
        internal DbSet<T> dbSet;

        public GenericRepository(AppDbContext context,ILogger logger)
        {
            _logger = logger;
            _context = context;
            dbSet = context.Set<T>();
        }

        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<bool> Delete(Guid id)
        {
            return false;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
            
        }

        public virtual async Task<T> GetBy(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public Task<bool> Updated(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
