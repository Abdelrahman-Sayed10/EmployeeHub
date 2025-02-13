using EmployeeHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Domain.Entities.Operation;

public class Department : AuditEntity<int>
{
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
