﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Contracts.IRepository;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
}
