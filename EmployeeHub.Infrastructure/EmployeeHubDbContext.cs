using EmployeeHub.Domain.Common;
using EmployeeHub.Domain.Entities.Operation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Infrastructure;

public class EmployeeHubDbContext : DbContext
{
    public EmployeeHubDbContext(DbContextOptions<EmployeeHubDbContext> options) : base(options) { }
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<User> Users => Set<User>();

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditEntity<int> || e.Entity is AuditEntity<Guid>);

        foreach (var entry in entries)
        {
            if(entry.Entity is AuditEntity<int> intAudit)
            {
                if(entry.State == EntityState.Added)
                {
                    intAudit.CreateDate = DateTime.UtcNow;
                    intAudit.CreatedBy = GetCurrentUser();
                }
                else if (entry.State == EntityState.Modified)
                {
                    intAudit.ModifyDate = DateTime.UtcNow;
                    intAudit.CreatedBy = GetCurrentUser();
                }
            }
            else if (entry.Entity is AuditEntity<Guid> guidAudit)
            {
                if(entry.State == EntityState.Added)
                {
                    guidAudit.CreateDate = DateTime.UtcNow;
                    guidAudit.CreatedBy = GetCurrentUser();
                }
                else if (entry.State == EntityState.Modified)
                {
                    guidAudit.ModifyDate = DateTime.UtcNow;
                    guidAudit.ModifiedBy = GetCurrentUser();
                }
            }
        }

        return base.SaveChanges();
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Similar auditing logic for async
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is AuditEntity<int> || e.Entity is AuditEntity<Guid>);

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditEntity<int> intAudit)
            {
                if (entry.State == EntityState.Added)
                {
                    intAudit.CreateDate = DateTime.UtcNow;
                    intAudit.CreatedBy = GetCurrentUser();
                }
                else if (entry.State == EntityState.Modified)
                {
                    intAudit.ModifyDate = DateTime.UtcNow;
                    intAudit.ModifiedBy = GetCurrentUser();
                }
            }
            else if (entry.Entity is AuditEntity<Guid> guidAudit)
            {
                if (entry.State == EntityState.Added)
                {
                    guidAudit.CreateDate = DateTime.UtcNow;
                    guidAudit.CreatedBy = GetCurrentUser();
                }
                else if (entry.State == EntityState.Modified)
                {
                    guidAudit.ModifyDate = DateTime.UtcNow;
                    guidAudit.ModifiedBy = GetCurrentUser();
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Should be completed to get the cuser from
    /// HttpContext
    /// </summary>
    /// <returns></returns>
    private string GetCurrentUser()
    {
        return "System";
    }
}
