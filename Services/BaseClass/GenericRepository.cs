using Microsoft.EntityFrameworkCore;
using rpg.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace rpg.Services.BaseClass
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> DbSet;
        public GenericRepository(DataContext context)
        {
            _context = context;
            DbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>()
            .Where(expression).AsNoTracking();

        }
        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
              _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
    
}
