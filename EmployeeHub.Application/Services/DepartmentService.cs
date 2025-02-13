using AutoMapper;
using EmployeeHub.Application.Contracts.IRepository;
using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Application.Exceptions;
using EmployeeHub.Domain.Entities.Operation;
using EmployeeHub.Dtos.DepartmentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }
    public async Task<DepartmentDto> AddDepartmentAsync(DepartmentAddDto departmentDto)
    {
        // 1) Convert DTO => Domain
        var departmentEntity = this.mapper.Map<Department>(departmentDto);

        // 2) Add
        var repo = this.unitOfWork.Repository<Department>();    
        await repo.AddAsync(departmentEntity);

        // 3) Commit
        await this.unitOfWork.SaveChangesAsync();

        // 4) Convert Domain back => DTO
        return this.mapper.Map<DepartmentDto>(departmentEntity);
    }

    public async Task DeleteDepartmentAsync(int id)
    {
        var repo = this.unitOfWork.Repository<Department>();    
        var department = await repo.GetByIdAsync(id);
        if (department == null) return;
        
        await repo.DeleteAsync(department);
        await this.unitOfWork.SaveChangesAsync();   
    }

    public async Task<List<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var repo = this.unitOfWork.Repository<Department>();
        var departments = await repo.GetAllAsync();

        //Map List<Department> to List<DepartmentDto>
        return this.mapper.Map<List<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        var repo = this.unitOfWork.Repository<Department>();

        var department = await repo.GetByIdAsync(id);     

        return department is not null
            ? this.mapper.Map<DepartmentDto>(department)
            : throw new NotFoundException($"Department with Id {id} was not found.");
    }

    public async Task UpdateDepartmentAsync(DepartmentDto departmentDto)
    {
        if (!departmentDto.Id.HasValue)
            throw new ArgumentNullException("Cannot update Department with no ID.");

        var repo = this.unitOfWork.Repository<Department>();
        var existingDepartment = await repo.GetByIdAsync(departmentDto.Id.Value);

        if (existingDepartment == null)
            return;

        existingDepartment.Description = departmentDto.Description;

        await repo.UpdateAsync(existingDepartment);
        await this.unitOfWork.SaveChangesAsync();
    }
}
