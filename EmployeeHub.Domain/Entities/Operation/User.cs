using EmployeeHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Domain.Entities.Operation;

public class User : AuditEntity<Guid>
{
    [Required]
    [MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}
