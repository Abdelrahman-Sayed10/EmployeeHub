using EmployeeHub.Dtos;
using EmployeeHub.Dtos.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Contracts.IService;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> AddAsync(EmployeeAddDto dto);
    Task UpdateAsync(EmployeeUpdateDto employeeDto);
    Task DeleteAsync(int id);

    Task<PagedResultDto<EmployeeDto>> GetPagedAsync(int pageIndex, int pageSize);
}
