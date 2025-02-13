using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Domain.Common;

public abstract class AuditEntity<T> : BaseEntity<T>
{
    
    public DateTime CreateDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifyDate { get; set; }
    public string? ModifiedBy { get; set; }
}
