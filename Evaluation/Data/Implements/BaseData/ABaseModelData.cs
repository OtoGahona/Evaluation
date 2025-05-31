using Entity.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Implements.BaseData
{
    public abstract class ABaseModelData<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected ABaseModelData(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Métodos abstractos que deben implementar las clases derivadas
        public abstract Task<List<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(int id);
        public abstract Task<T> CreateAsync(T entity);
        public abstract Task<T> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id);
    }
}