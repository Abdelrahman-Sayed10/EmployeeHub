using EmployeeHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Domain.Entities.Operation;

public class Employee : AuditEntity<int>
{
    [Required]
    [MaxLength(200)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = default!;
}
