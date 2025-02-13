using EmployeeHub.Application.Contracts.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly EmployeeHubDbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories = new(); 

    public UnitOfWork(EmployeeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves (or creates) a repository for type T, caching it so we don’t create multiple instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IRepository<T> Repository<T>() where T : class
    {
        var entityType = typeof(T);
        if (!_repositories.TryGetValue(entityType, out var repo))
        {
            repo = new Repository<T>(_dbContext);
            _repositories[entityType] = repo;
        }
        return (IRepository<T>)repo!;
    }

    /// <summary>
    /// Commits all changes made by any repository to the database in one transaction
    /// </summary>
    /// <returns></returns>
    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
