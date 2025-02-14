using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Contracts.IRepository;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id);   
    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);    
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    // Overload to apply eager loading with include
    Task<IEnumerable<T>> GetAllAsync(
        params Expression<Func<T, object>>[] includes
    );
}
