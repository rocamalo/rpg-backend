using System.Linq.Expressions;

namespace rpg.Services.BaseClass
{
    public interface IGenericRepository<T> where T : class
    {
       

        IQueryable<T> GetAll();

        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);

        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
         

    }
}
