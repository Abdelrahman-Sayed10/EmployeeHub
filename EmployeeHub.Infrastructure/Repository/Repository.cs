using EmployeeHub.Application.Contracts.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly EmployeeHubDbContext _dbSContext;
    private readonly DbSet<T> _dbSet; 

    public Repository(EmployeeHubDbContext dbSContext)
    {
        _dbSContext = dbSContext;
        //Instead of calling _dbContext.Set<T>() every time
        //better to store it once in to reduce the overhead
        _dbSet = _dbSContext.Set<T>();
    }
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);  
        //SaveChanges will be handled by UnitOfWork
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);

        //To avoid compiler warnings on async method without await
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);  
        //To avoid compiler warnings on async method without await
        await Task.CompletedTask;   
    }

    async Task<IEnumerable<T>> IRepository<T>.GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        // Base Query
        IQueryable<T> query = _dbSet;

        // Apply each include expression
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        // Fetch
        return await query.ToListAsync();
    }
}
