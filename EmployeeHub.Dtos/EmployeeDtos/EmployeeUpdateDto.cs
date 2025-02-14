using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Dtos.EmployeeDtos;

public class EmployeeUpdateDto
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }

    [Required(ErrorMessage = "FirstName is required")]
    [StringLength(200, ErrorMessage = "Max length is 200 chars")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required")]
    [StringLength(200, ErrorMessage = "Max length is 200 chars")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [Required(ErrorMessage = "Department is required")]
    public int DepartmentId { get; set; }
}
