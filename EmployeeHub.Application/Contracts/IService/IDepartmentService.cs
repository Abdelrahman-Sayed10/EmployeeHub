using EmployeeHub.Dtos.DepartmentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Contracts.IService;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllDepartmentsAsync();
    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
    Task<DepartmentDto> AddDepartmentAsync(DepartmentAddDto departmentDto);
    Task UpdateDepartmentAsync(DepartmentDto departmentDto);
    Task DeleteDepartmentAsync(int id);
}
