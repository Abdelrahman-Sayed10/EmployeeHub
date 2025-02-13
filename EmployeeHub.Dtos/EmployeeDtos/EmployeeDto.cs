﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Dtos.EmployeeDtos;

public class EmployeeDto
{
    public int? Id { get; set; }          
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public string DepartmentDescription { get; set; } = string.Empty;
}
