using AutoMapper;
using EmployeeHub.Application.Contracts.IRepository;
using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Application.Exceptions;
using EmployeeHub.Domain.Entities.Operation;
using EmployeeHub.Dtos;
using EmployeeHub.Dtos.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<EmployeeDto> AddAsync(EmployeeAddDto dto)
    {
        // 1) Map EmployeeAddDto => Employee
        var employeeEntity = this.mapper.Map<Employee>(dto);

        // 2) Save
        var departmentRepo = this.unitOfWork.Repository<Department>();
        var existingDepartment = await departmentRepo.GetByIdAsync(dto.DepartmentId);
        if (existingDepartment == null)
            throw new NotFoundException($"Department not found with ID = {dto.DepartmentId}.");

        var repo = this.unitOfWork.Repository<Employee>();
        await repo.AddAsync(employeeEntity);
        await this.unitOfWork.SaveChangesAsync();

        // 3) Map Employee => EmployeeDto   
        return this.mapper.Map<EmployeeDto>(employeeEntity);

    }

    public async Task DeleteAsync(int id)
    {
        var repo = this.unitOfWork.Repository<Employee>();
        var employee = await repo.GetByIdAsync(id);
        if (employee == null)
            throw new NotFoundException($"Employee not found with ID = {id}.");

        await repo.DeleteAsync(employee);
        await this.unitOfWork.SaveChangesAsync();
    }

    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        var repo = this.unitOfWork.Repository<Employee>();
        var employees = await repo.GetAllAsync(e => e.Department);

        // Map domain -> EmployeeDto
        return this.mapper.Map<List<EmployeeDto>>(employees);
    }   

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var repo = this.unitOfWork.Repository<Employee>();
        var employees = await repo.GetAllAsync(e => e.Department);
        var employee = employees.FirstOrDefault(e => e.Id == id); 
        if(employee == null)
            throw new NotFoundException($"Employee not found with ID = {id}.");

        return this.mapper.Map<EmployeeDto>(employee);
    }

    public async Task<PagedResultDto<EmployeeDto>> GetPagedAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 1;

        var repo = this.unitOfWork.Repository<Employee>();

        var employees = await repo.GetAllAsync(e => e.Department);  

        var totalCount = employees.Count();
        var pagedEmployees = employees
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dataDtos = this.mapper.Map<List<EmployeeDto>>(pagedEmployees);

        return new PagedResultDto<EmployeeDto>
        {
            Data = dataDtos,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task UpdateAsync(EmployeeUpdateDto employeeDto)
    {
        var repo = this.unitOfWork.Repository<Employee>();
        var existingEmployee = await repo.GetByIdAsync(employeeDto.Id);

        if (existingEmployee == null)
            throw new NotFoundException($"Employee not found with ID = {employeeDto.Id}.");
    }
}
